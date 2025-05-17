namespace Chatter.Shared.DomainTypes;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}