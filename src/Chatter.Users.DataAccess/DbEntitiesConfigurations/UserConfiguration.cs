using Core.DataAccessTypes;
using Chatter.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatter.Users.DataAccess.DbEntitiesConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .WithId()
            .WithAuditable()
            .WithSoftDeletable()
            .ToTable("Users");

        builder.Property(x => x.KeycloakId);
        builder.Property(x => x.UserName);
        builder.Property(x => x.Email);
    }
}