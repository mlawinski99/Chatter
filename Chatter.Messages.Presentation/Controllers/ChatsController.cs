using Microsoft.AspNetCore.Mvc;

namespace Chatter.MessagesService.Controllers;


[ApiController]
[Route("[controller]")]
public class ChatsController
{
    private readonly ILogger<MessagesController> _logger;

    public ChatsController(ILogger<MessagesController> logger)
    {
        _logger = logger;
    }
}