using Chatter.MessagesDomain;
using Core.DataAccessTypes;
using Core.DomainTypes;
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
        
        builder.HasMany(x => x.Members);
    }
}