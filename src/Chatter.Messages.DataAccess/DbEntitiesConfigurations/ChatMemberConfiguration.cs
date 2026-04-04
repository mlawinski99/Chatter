using Chatter.MessagesDomain;
using Core.DataAccessTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatter.MessagesDataAccess.DbEntitiesConfigurations;

public class ChatMemberConfiguration : IEntityTypeConfiguration<ChatMember>
{
    public void Configure(EntityTypeBuilder<ChatMember> builder)
    {
        builder
            .WithId()
            .WithAuditableWithUser()
            .WithSoftDeletable()
            .ToTable("ChatMembers", "chat");

        builder.HasOne(x => x.Chat);
        builder.HasOne(x => x.User);
    }
}