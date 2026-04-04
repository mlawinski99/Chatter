using Chatter.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chatter.Shared.Context;

public interface IUserContext
{
    DbSet<KeycloakAdminEvent> KeycloakAdminEvents { get; set; }
    DbSet<User> Users { get; set; }
}