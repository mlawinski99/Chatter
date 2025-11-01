using Chatter.MessagesDomain;
using Chatter.Shared.Encryption.JsonSerializable;
using Chatter.SyncUsersJob.Models;
using Microsoft.EntityFrameworkCore;

namespace Chatter.SyncUsersJob;

public class SyncDbContext : DbContext
{
    public SyncDbContext(DbContextOptions<SyncDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<ConfigurationData> ConfigurationData { get; set; }
    public DbSet<KeycloakAdminEvent> KeycloakAdminEvents { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ConfigurationData>().HasKey(x => x.Key);
        modelBuilder.Entity<ConfigurationData>()
            .Property(x => x.Value);
    }
}