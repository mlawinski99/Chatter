using Chatter.Shared.DomainTypes;

namespace Chatter.IntegrationTests.Shared.Infrastructure.TestEntities;

public class AuditableWithUserEntity : Entity, IAuditableWithUser
{
    public string Name { get; set; } = string.Empty;
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? ModifiedBy { get; set; }
}
