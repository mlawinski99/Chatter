using Chatter.Shared.DomainTypes;

namespace Chatter.Shared.Domain;

//@TODO publish event after keycloak change user outbox, sync with keycloak
public class User : Entity, IAuditable, ISoftDeletable
{
    [Encryptable]
    public string UserName { get; set; }
    [Encryptable]
    public string Email { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public bool IsDeleted { get; set; }
}