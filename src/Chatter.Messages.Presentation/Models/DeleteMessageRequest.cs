namespace Chatter.MessagesService.Models;

public class DeleteMessageRequest
{
    public Guid ChatId { get; set; }
    public Guid MessageId { get; set; }
}