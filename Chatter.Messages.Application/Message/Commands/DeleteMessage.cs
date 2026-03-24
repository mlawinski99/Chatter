using Chatter.Messages.Application.Message.Errors;
using Chatter.MessagesDataAccess.DbContexts;
using Chatter.Shared.CQRS;
using Chatter.Shared.DataAccessTypes;
using Chatter.Shared.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace Chatter.Messages.Application.Message.Commands;

public class DeleteMessage : ICommandHandler<DeleteMessage.DeleteMessageCommand, Result>
{
    public record DeleteMessageCommand(Guid ChatId, Guid MessageId) : ICommand<Result>;

    private readonly ChatDbContext _chatDbContext;
    private readonly IUserProvider _userProvider;

    public DeleteMessage(ChatDbContext chatDbContext,
        IUserProvider userProvider)
    {
        _chatDbContext = chatDbContext;
        _userProvider = userProvider;
    }

    public async Task<Result> Handle(DeleteMessageCommand model, CancellationToken cancellationToken)
    {
        var message = await _chatDbContext.Messages
            .FirstOrDefaultAsync(x => x.Id == model.MessageId, cancellationToken);

        if (message is null)
            return Result.NotFound(ErrorMessages.MessageNotFound);

        if (message.ChatId != model.ChatId)
            return Result.BadRequest(ErrorMessages.MessageDoesNotBelongToChat);

        if (message.CreatedBy != _userProvider.UserId)
            return Result.Forbidden(ErrorMessages.CanOnlyDeleteOwnMessages);

        message.Delete();
        _chatDbContext.Messages.Remove(message);
        await _chatDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}