namespace Chatter.Shared.DomainTypes;

public interface ISoftDeletable
{
    DateTime? DateDeletedUtc { get; set; }
    bool IsDeleted { get; }
}