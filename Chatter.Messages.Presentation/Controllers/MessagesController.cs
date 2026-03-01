using Chatter.Messages.Application.Message.Commands;
using Chatter.MessagesService.Models;
using Chatter.Shared.CQRS;
using Chatter.Shared.ResultPattern;
using Chatter.Shared.Web;
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
    
    // [HttpPut]
    // public IActionResult EditMessage(EditMessageRequest model)
    // {
    //     return Ok();
    // }
    //
    [HttpDelete]
    public IActionResult RemoveMessage(Guid messageId)
    {
        return Ok();
    }
    
    [HttpGet]
    public IActionResult LoadMessages(string groupId, string lastMessageId)
    {
        return Ok();
    }
}