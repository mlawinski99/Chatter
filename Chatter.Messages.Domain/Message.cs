using Chatter.MessagesDomain.Events;
using Chatter.Shared.DomainTypes;

namespace Chatter.MessagesDomain;

public class Message : AggregateRoot, IAuditableWithUser, ISoftDeletable, IVersionable
{
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
    public MessageStatus Status { get; set; }
    public MessageContent Content { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public bool IsDeleted { get; set; }
    public int VersionId { get; set; }
    public Guid? VersionGroupId { get; set; }
    public User Sender { get; set; }
    public Chat Chat { get; set; }
    private Message() { } // for EF
    public Message(MessageContent content, User sender, Chat chat)
    {
        Status = MessageStatus.Sending;
        Content = content;
        Sender = sender;
        Chat = chat;
        AddDomainEvent(new MessageCreated(chat.Id, content.Text, sender.Id));
    }
}