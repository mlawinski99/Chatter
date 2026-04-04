using Core.DomainTypes;

namespace Chatter.IntegrationTests.Shared.Infrastructure.TestEntities;

public class SoftDeletableEntity : Entity, ISoftDeletable
{
    public string Name { get; set; } = string.Empty;
    public DateTime? DateDeletedUtc { get; set; }
    public bool IsDeleted { get; set; }
}
