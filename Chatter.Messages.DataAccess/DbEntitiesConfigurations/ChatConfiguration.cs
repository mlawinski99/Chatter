using Chatter.MessagesDomain;
using Chatter.Shared.DataAccessTypes;
using Chatter.Shared.DomainTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatter.MessagesDataAccess.DbEntitiesConfigurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder
            .WithId()
            .WithAuditable()
            .ToTable("Chats", "chat");
        
        builder.Property(x => x.Type)
            .HasConversion(
                x => x.Name, 
                x => Enumeration.GetByName<ChatType>(x));
        
        builder.HasMany(x => x.Messages);
        builder.HasMany(x => x.Members);
    }
}