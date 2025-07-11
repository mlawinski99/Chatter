using Chatter.Shared.DomainTypes;

namespace Chatter.MessagesDomain;

public class MessageContent : ValueObject
{
    public string Text { get; }

    public MessageContent(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Message cannot be empty.");

        if (text.Length > 1000)
            throw new ArgumentException("Message is too long.");

        Text = text;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Text;
    }
}