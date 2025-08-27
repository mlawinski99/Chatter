using Chatter.MessagesDataAccess.DbContexts;
using Chatter.MessagesDomain;
using Chatter.Shared.CQRS;
using Chatter.Shared.DataAccessTypes;
using Chatter.Shared.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace Chatter.Messages.Application.Message.Commands;

public class SendMessage : ICommandHandler<SendMessage.SendMessageCommand, Result>
{
    public record SendMessageCommand(Guid ChatId, string Content) : ICommand<Result>;
 
    private readonly ChatDbContext _chatDbContext;
    private readonly IUserProvider _userProvider;

    public SendMessage(ChatDbContext chatDbContext,
        IUserProvider userProvider)
    {
        _chatDbContext = chatDbContext;
        _userProvider = userProvider;
    }
    public async Task<Result> Handle(SendMessageCommand model, CancellationToken cancellationToken)
    {
        var chat = await _chatDbContext.Chats
            .FirstOrDefaultAsync(x => x.Id == model.ChatId, cancellationToken);

        if (chat is null)
            return Result.Failure("Chat not found");

        var message = new MessagesDomain.Message(
            new MessageContent(model.Content),
            new User { Id = (Guid)_userProvider.UserId },
            chat);
        
        await _chatDbContext.Messages.AddAsync(message, cancellationToken);
        await _chatDbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success;
    }
}