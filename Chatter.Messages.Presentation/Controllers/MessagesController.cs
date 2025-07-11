using Chatter.MessagesService.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chatter.MessagesService.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(ILogger<MessagesController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult SendMessage(SendMessageRequest model)
    {
        return Ok();
    }
    
    [HttpPut]
    public IActionResult EditMessage(EditMessageRequest model)
    {
        return Ok();
    }
    
    [HttpGet]
    public IActionResult GetMoreMessages(string groupId, string lastMessageId)
    {
        return Ok();
    }
    
    [HttpGet]
    public IActionResult GetGroupWithLastMessages(string gropuId)
    {
        return Ok();
    }
}