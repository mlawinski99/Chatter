using Chatter.Shared.Context;
using Chatter.Shared.DataAccessTypes;
using Chatter.Shared.Domain;
using Chatter.Shared.Encryption.JsonSerializable;
using Chatter.Users.DataAccess.DbEntitiesConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chatter.Users.DataAccess.DbContexts;

public class UsersDbContext : BaseDbContext, IUserContext, IConfigurationContext
{
    public DbSet<ConfigurationData> ConfigurationData { get; set; }
    public DbSet<KeycloakAdminEvent> KeycloakAdminEvents { get; set; }
    public DbSet<User> Users { get; set; }

    public UsersDbContext(DbContextOptions<UsersDbContext> options,
        IJsonSerializer jsonSerializer,
        IEnumerable<IInterceptor> interceptors) : base(options, jsonSerializer, interceptors)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new KeycloakAdminEventConfiguration());
        modelBuilder.ApplyConfiguration(new ConfigurationDataConfiguration());
    }
}