using Chatter.IntegrationTests.Shared.Infrastructure.TestEntities;
using Chatter.Shared.Context;
using Core.DataAccessTypes;
using Chatter.Shared.Domain;
using Core.Infrastructure.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chatter.IntegrationTests.Shared.Infrastructure;

public class TestDbContext : BaseDbContext, IUserContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options, IJsonSerializer jsonSerializer, IEnumerable<IInterceptor> interceptors)
        : base(options, jsonSerializer, interceptors)
    {
    }

    public DbSet<KeycloakAdminEvent> KeycloakAdminEvents { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ConfigurationData> ConfigurationData { get; set; }
    public DbSet<AuditableEntity> AuditableEntities { get; set; }
    public DbSet<AuditableWithUserEntity> AuditableWithUserEntities { get; set; }
    public DbSet<SoftDeletableEntity> SoftDeletableEntities { get; set; }
    public DbSet<VersionableEntity> VersionableEntities { get; set; }
    public DbSet<EncryptableEntity> EncryptableEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<KeycloakAdminEvent>(entity =>
        {
            entity.ToTable("KeycloakAdminEvents");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.OperationType).HasMaxLength(20);
            entity.Property(e => e.ResourceType).HasMaxLength(50);
            entity.Property(e => e.ResourcePath).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserName).HasMaxLength(1024);
            entity.Property(e => e.Email).HasMaxLength(1024);
        });

        modelBuilder.Entity<ConfigurationData>(entity =>
        {
            entity.ToTable("ConfigurationData");
            entity.HasKey(e => e.Key);
        });

        modelBuilder.Entity<AuditableEntity>(entity =>
        {
            entity.ToTable("AuditableEntities");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<AuditableWithUserEntity>(entity =>
        {
            entity.ToTable("AuditableWithUserEntities");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<SoftDeletableEntity>(entity =>
        {
            entity.ToTable("SoftDeletableEntities");
            entity.HasKey(e => e.Id);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<VersionableEntity>(entity =>
        {
            entity.ToTable("VersionableEntities");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<EncryptableEntity>(entity =>
        {
            entity.ToTable("EncryptableEntities");
            entity.HasKey(e => e.Id);
        });
    }
}
