using Chatter.Shared.DomainTypes;

namespace Chatter.MessagesDomain;


public class ChatMember : Entity, IAuditableWithUser, ISoftDeletable
{
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string CreatedBy { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? DateDeletedUtc { get; set; } = null;
    public bool IsDeleted { get; set; } = false;
    
    public Chat Chat { get; set; }
    public User User { get; set; }

    public ChatMember(User user, Chat chat)
    {
        User = user;
        Chat = chat;
    }

}