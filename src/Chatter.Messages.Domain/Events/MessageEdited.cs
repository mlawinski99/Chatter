using Core.DomainTypes;

namespace Chatter.MessagesDomain.Events;

public class MessageEdited : DomainEventBase
{
    public Guid MessageId { get; private set; }
    public Guid ChatId { get; private set; }
    public string NewContent { get; private set; }

    public MessageEdited(Guid messageId, Guid chatId, string newContent)
    {
        MessageId = messageId;
        ChatId = chatId;
        NewContent = newContent;
    }
}
