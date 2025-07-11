using Chatter.MessagesDomain;
using Chatter.Shared.DataAccessTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatter.MessagesDataAccess.DbEntitiesConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .WithId()
            .WithAuditable()
            .WithSoftDeletable()
            .ToTable("dbo.Users");
        
        builder.Property(x => x.UserName);
        builder.Property(x => x.Email);
    }
}