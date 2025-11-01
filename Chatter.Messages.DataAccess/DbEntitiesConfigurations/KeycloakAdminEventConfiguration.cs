using Chatter.MessagesDomain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatter.MessagesDataAccess.DbEntitiesConfigurations;

public class KeycloakAdminEventConfiguration : IEntityTypeConfiguration<KeycloakAdminEvent>
{
    public void Configure(EntityTypeBuilder<KeycloakAdminEvent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OperationType).HasMaxLength(20);
        builder.Property(x => x.ResourceType).HasMaxLength(50);
        builder.Property(x => x.ResourcePath).HasMaxLength(255);
        builder.Property(x => x.Time);
        builder.Property(x => x.IsProcessed);
    }
}