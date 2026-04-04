using Chatter.Messages.Application.Message.Commands;
using Chatter.Messages.Application.Message.Queries;
using Chatter.MessagesService.Models;
using Core.CQRS;
using Core.Pager;
using Core.ResultPattern;
using Core.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chatter.MessagesService.Controllers;

[Route("[controller]")]
[Authorize]
public class MessagesController(IRequestDispatcher requestDispatcher) : BaseController(requestDispatcher)
{
    [HttpPost]
    public async Task<Result> SendMessage(SendMessageRequest model)
    {
        var request = new SendMessage.SendMessageCommand(model.ChatId, model.Content);

        return await _requestDispatcher.Dispatch(request);
    }

    [HttpPut]
    public async Task<Result> EditMessage(EditMessageRequest model)
    {
        var request = new EditMessage.EditMessageCommand(model.ChatId, model.MessageId, model.Content);

        return await _requestDispatcher.Dispatch(request);
    }

    [HttpDelete]
    public async Task<Result> DeleteMessage(DeleteMessageRequest model)
    {
        var request = new DeleteMessage.DeleteMessageCommand(model.ChatId, model.MessageId);

        return await _requestDispatcher.Dispatch(request);
    }

    [HttpGet]
    public async Task<Result<CursorPagedResult<LoadMessages.MessageDto>>> LoadMessages(
        Guid chatId, Guid? lastMessageId, int pageSize = 20)
    {
        var request = new LoadMessages.LoadMessagesQuery(chatId, lastMessageId, pageSize);

        return await _requestDispatcher.Dispatch(request);
    }
}