using Chatter.Shared.DomainTypes;

namespace Chatter.IntegrationTests.Shared.Infrastructure.TestEntities;

public class VersionableEntity : Entity, IVersionable
{
    public string Name { get; set; } = string.Empty;
    public int VersionId { get; set; }
    public Guid? VersionGroupId { get; set; }
}
