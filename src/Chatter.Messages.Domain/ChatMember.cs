using Chatter.Shared.Domain;
using Core.DomainTypes;

namespace Chatter.MessagesDomain;


public class ChatMember : Entity, IAuditableWithUser, ISoftDeletable
{
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? DateDeletedUtc { get; set; } = null;
    public bool IsDeleted { get; set; } = false;
    
    public Chat Chat { get; set; }
    public User User { get; set; }
    
    public static ChatMember Create(User user, Chat chat)
    {
        var chatMember = new ChatMember();
        chatMember.User = user;
        chatMember.Chat = chat;
        
        return chatMember;
    }

}