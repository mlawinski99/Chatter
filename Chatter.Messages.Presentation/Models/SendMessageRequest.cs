using Chatter.Shared.CQRS;

namespace Chatter.MessagesService.Models;

public class SendMessageRequest
{
    public string Content { get; set; }
    public Guid ChatId { get; set; }
}