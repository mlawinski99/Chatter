namespace Chatter.Shared.DomainTypes;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}