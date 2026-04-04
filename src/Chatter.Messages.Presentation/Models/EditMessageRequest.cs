namespace Chatter.MessagesService.Models;

public class EditMessageRequest
{
    public Guid ChatId { get; set; }
    public Guid MessageId { get; set; }
    public string Content { get; set; }
}