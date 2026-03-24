using Chatter.Messages.Application.Message.Errors;
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
        var chatExists = await _chatDbContext.Chats
            .AnyAsync(x => x.Id == model.ChatId, cancellationToken);

        if (!chatExists)
            return Result.NotFound(ErrorMessages.ChatNotFound);

        var message = MessagesDomain.Message.Create(
            MessageContent.Create(model.Content),
            (Guid)_userProvider.UserId!,
            model.ChatId);

        await _chatDbContext.Messages.AddAsync(message, cancellationToken);
        await _chatDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}