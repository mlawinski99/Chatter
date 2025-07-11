namespace Chatter.Shared.DomainTypes;

public class MessageStatus(int id, string name) : Enumeration(id, name)
{
    public static MessageStatus Sending = new(1, nameof(Sending));
    public static MessageStatus Sent = new(2, nameof(Sent));
    public static MessageStatus SentFailed = new(3, nameof(SentFailed));
}