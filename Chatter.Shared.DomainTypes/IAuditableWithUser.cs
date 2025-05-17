namespace Chatter.Shared.DomainTypes;

public interface IAuditableWithUser : IAuditable
{
    string CreatedBy { get; set; }
    string ModifiedBy { get; set; }
}