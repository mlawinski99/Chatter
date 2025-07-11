using Chatter.Shared.DomainTypes;

namespace Chatter.MessagesDomain;

public class Message : AggregateRoot, IAuditableWithUser, ISoftDeletable, IVersionable
{
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string CreatedBy { get; set; }
    public string ModifiedBy { get; set; }
    public MessageStatus Status { get; set; }
    public MessageContent Content { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public bool IsDeleted { get; set; }
    public int VersionId { get; set; }
    public Guid? VersionGroupId { get; set; }
    public User Sender { get; set; }
    public Chat Chat { get; set; }

    public Message(MessageContent content, User sender)
    {
        Status = MessageStatus.Sending;
        Content = content;
        Sender = sender;
    }
}