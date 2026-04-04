using Core.DomainTypes;

namespace Chatter.IntegrationTests.Shared.Infrastructure.TestEntities;

public class AuditableEntity : Entity, IAuditable
{
    public string Name { get; set; } = string.Empty;
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
}
