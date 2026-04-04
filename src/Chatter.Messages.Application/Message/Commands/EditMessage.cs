using Core.Infrastructure;
using Chatter.Messages.Application.Message.Errors;
using Chatter.MessagesDataAccess.DbContexts;
using Chatter.MessagesDomain;
using Core.CQRS;
using Core.DataAccessTypes;
using Core.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace Chatter.Messages.Application.Message.Commands;

public class EditMessage : ICommandHandler<EditMessage.EditMessageCommand, Result>
{
    public record EditMessageCommand(Guid ChatId, Guid MessageId, string Content) : ICommand<Result>;

    private readonly ChatDbContext _chatDbContext;
    private readonly IUserProvider _userProvider;

    public EditMessage(ChatDbContext chatDbContext,
        IUserProvider userProvider)
    {
        _chatDbContext = chatDbContext;
        _userProvider = userProvider;
    }

    public async Task<Result> Handle(EditMessageCommand model, CancellationToken cancellationToken)
    {
        var message = await _chatDbContext.Messages
            .FirstOrDefaultAsync(x => x.Id == model.MessageId, cancellationToken);

        if (message is null)
            return Result.NotFound(ErrorMessages.MessageNotFound);

        if (message.ChatId != model.ChatId)
            return Result.BadRequest(ErrorMessages.MessageDoesNotBelongToChat);

        if (message.CreatedBy != _userProvider.UserId)
            return Result.Forbidden(ErrorMessages.CanOnlyEditOwnMessages);

        message.Edit(MessageContent.Create(model.Content));
        await _chatDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
