using Chatter.Shared.DomainTypes;

namespace Chatter.MessagesDomain.Events;

public class MessageDeleted : DomainEventBase
{
    public Guid MessageId { get; private set; }
    public Guid ChatId { get; private set; }

    public MessageDeleted(Guid messageId, Guid chatId)
    {
        MessageId = messageId;
        ChatId = chatId;
    }
}