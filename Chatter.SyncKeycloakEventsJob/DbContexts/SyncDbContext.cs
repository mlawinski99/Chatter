using Chatter.Shared.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chatter.SyncKeycloakEvents.DbContexts;

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

        modelBuilder.Entity<ConfigurationData>(entity =>
        {
            entity.ToTable("ConfigurationData");
            entity.HasKey(x => x.Key);
            entity.Property(x => x.Value);
        });

        modelBuilder.Entity<KeycloakAdminEvent>(entity =>
        {
            entity.ToTable("KeycloakAdminEvents");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
        });
    }
}