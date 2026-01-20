using Chatter.MessagesDomain;
using Chatter.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatter.MessagesDataAccess.DbEntitiesConfigurations;

public class ConfigurationDataConfiguration : IEntityTypeConfiguration<ConfigurationData>
{
    public void Configure(EntityTypeBuilder<ConfigurationData> builder)
    {
        builder.HasKey(x => x.Key);
        builder.Property(x => x.Value);
    }
}