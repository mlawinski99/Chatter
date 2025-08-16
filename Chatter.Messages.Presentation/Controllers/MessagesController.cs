using Chatter.Messages.Application.Message.Commands;
using Chatter.MessagesService.Models;
using Chatter.Shared.CQRS;
using Chatter.Shared.ResultPattern;
using Microsoft.AspNetCore.Mvc;

namespace Chatter.MessagesService.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IRequestDispatcher _requestDispatcher;

    public MessagesController(IRequestDispatcher requestDispatcher)
    {
        _requestDispatcher = requestDispatcher;
    }

    [HttpPost]
    public async Task<Result<object?>> SendMessage(SendMessageRequest model)
    { 
        var request = new SendMessage.SendMessageCommand(model.ChatId, model.Content);

        return await _requestDispatcher.Dispatch(request);
    }
    
    [HttpPut]
    public IActionResult EditMessage(EditMessageRequest model)
    {
        return Ok();
    }
    
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