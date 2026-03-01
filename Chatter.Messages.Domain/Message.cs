using Chatter.MessagesDomain.Events;
using Chatter.Shared.Domain;
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
    
    public static Message Create(MessageContent content, User sender, Chat chat)
    {
        var message = new Message();
        message.Status = MessageStatus.Sending;
        message.Content = content;
        message.Sender = sender;
        message.Chat = chat;
        message.AddDomainEvent(new MessageCreated(chat.Id, content.Text, sender.Id));

        return message;
    }
}