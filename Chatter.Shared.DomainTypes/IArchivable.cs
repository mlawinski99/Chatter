namespace Chatter.Shared.DomainTypes;

public interface IArchivable
{
    bool IsArchived { get; set; }
    DateTime? DateArchivedUtc { get; set; }
}