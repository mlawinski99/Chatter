using Chatter.Shared.Domain;
using Core.DomainTypes;

namespace Chatter.MessagesDomain;

public class Chat : AggregateRoot, IAuditable
{
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public ChatType Type { get; set; }

    private readonly List<ChatMember> _members = new();
    public IReadOnlyCollection<ChatMember> Members => _members.AsReadOnly();

    public static Chat Create(ChatType type)
    {
        var chat = new Chat();
        chat.Type = type;

        return chat;
    }

    public void AddMember(User user)
    {
        if(_members.Any(x => x.User.Id == user.Id))
            throw new DomainException("Member is already in the chat.");

        _members.Add(ChatMember.Create(user, this));
    }

    public void AddMembers(List<User> users)
    {
        foreach(var user in users)
            AddMember(user);
    }

    public void RemoveMember(User user)
    {
        var member = _members.FirstOrDefault(x => x.User.Id == user.Id);
        if (member is null)
            throw new DomainException("Member is not in the chat.");
        if (member.IsDeleted)
            throw new DomainException("Member is already deleted.");

        _members.Remove(member);
    }

    public void RemoveMembers(List<User> users)
    {
        foreach(var user in users)
            RemoveMember(user);
    }
}