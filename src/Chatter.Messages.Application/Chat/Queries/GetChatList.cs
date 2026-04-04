using Core.Infrastructure;
using Chatter.MessagesDataAccess.DbContexts;
using Core.CQRS;
using Core.DataAccessTypes;
using Core.Pager;
using Core.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace Chatter.Messages.Application.Chat.Queries;

public class GetChatList : IQueryHandler<GetChatList.GetChatListQuery, Result<PagedResult<GetChatList.ChatDto>>>
{
    public record LastMessageDto(string Content, DateTime DateCreatedUtc);
    public record ChatDto(Guid Id, string Type, DateTime DateCreatedUtc, LastMessageDto? LastMessage);
    public record GetChatListQuery(int Page = 1, int PageSize = 10) : IQuery<Result<PagedResult<ChatDto>>>;

    private readonly ChatDbContext _chatDbContext;
    private readonly IUserProvider _userProvider;

    public GetChatList(ChatDbContext chatDbContext, IUserProvider userProvider)
    {
        _chatDbContext = chatDbContext;
        _userProvider = userProvider;
    }

    public async Task<Result<PagedResult<ChatDto>>> Handle(GetChatListQuery query, CancellationToken cancellationToken)
    {
        var userChats = _chatDbContext.ChatMembers
            .AsNoTracking()
            .Where(cm => cm.User.Id == _userProvider.UserId)
            .Select(cm => cm.Chat);

        var totalCount = await userChats.CountAsync(cancellationToken);

        var chats = await userChats
            .Select(chat => new
            {
                chat.Id,
                TypeName = chat.Type.Name,
                chat.DateCreatedUtc,
                LastMessage = _chatDbContext.Messages
                    .Where(m => m.ChatId == chat.Id)
                    .OrderByDescending(m => m.DateCreatedUtc)
                    .Select(m => new { m.Content.Text, m.DateCreatedUtc })
                    .FirstOrDefault()
            })
            .OrderByDescending(c => c.LastMessage != null ? c.LastMessage.DateCreatedUtc : c.DateCreatedUtc)
            .Paginate(query.Page, query.PageSize)
            .Select(c => new ChatDto(c.Id, c.TypeName, c.DateCreatedUtc,
                c.LastMessage != null ? new LastMessageDto(c.LastMessage.Text, c.LastMessage.DateCreatedUtc) : null))
            .ToListAsync(cancellationToken);

        var result = PagedResult<ChatDto>.Create(chats, query.Page, query.PageSize, totalCount);
        
        return Result<PagedResult<ChatDto>>.Success(result);
    }
}
