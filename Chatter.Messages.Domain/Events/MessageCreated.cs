using Chatter.Shared.DomainTypes;

namespace Chatter.MessagesDomain.Events;

public class MessageCreated : DomainEventBase
{
    public Guid ChatId { get; private set; }
    public string Content { get; private set; }
    public Guid AuthorId { get; private set; }

    public MessageCreated(Guid chatId, string content, Guid authorId)
    {
        ChatId = chatId;
        Content = content;
        AuthorId = authorId;
    }
}