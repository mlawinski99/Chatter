using Chatter.Shared.DomainTypes;

namespace Chatter.MessagesDomain;

public class Chat : AggregateRoot, IAuditable
{
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public List<Message> Messages { get; set; } = new();
    public ChatType Type { get; set; }
    
    private readonly List<ChatMember> _members = new();
    public IReadOnlyCollection<ChatMember> Members => _members.AsReadOnly();
    private Chat() { } // for EF
    public Chat(ChatType type)
    {
        Type = type;
    }

    public void AddMember(User user)
    {
        if(_members.Any(x => x.User == user))
            throw new ApplicationException("Member is already in the chat.");
        
        _members.Add(new ChatMember(user, this));
    }

    public void AddMembers(List<User> users)
    {
        foreach(var user in users)
            AddMember(user);
    }

    public void RemoveMember(User user)
    {
        var member = _members.FirstOrDefault(x => x.User == user);
        if (member is null)
            throw new ApplicationException("Member is not in the chat.");
        if (member.IsDeleted)
            throw new ApplicationException("Member is already deleted.");
        
        _members.Remove(member);
    }
    
    public void RemoveMembers(List<User> users)
    {
        foreach(var user in users)
            RemoveMember(user);
    }

    public void AddMessage(Message message)
    {
        this.Messages.Add(message);
    }

    public void RemoveMessage(Message message)
    {
        this.Messages.Remove(message);
    }
}