using Chatter.Shared.DomainTypes;

namespace Chatter.MessagesDomain;

public class ChatType(int id, string name) : Enumeration(id, name)
{
    public static ChatType Private = new(1, nameof(Private));
    public static ChatType Group = new(2, nameof(Group));
}