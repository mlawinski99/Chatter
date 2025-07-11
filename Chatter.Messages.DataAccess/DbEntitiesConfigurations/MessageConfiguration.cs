using Chatter.MessagesDomain;
using Chatter.Shared.DataAccessTypes;
using Chatter.Shared.DomainTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatter.MessagesDataAccess.DbEntitiesConfigurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder
            .WithId()
            .WithAuditableWithUser()
            .WithSoftDeletable()
            .WithVersionable()
            .ToTable("chat.Messages");
        builder.OwnsOne(m => m.Content, navBuilder =>
        {
            navBuilder.Property(c => c.Text)
                .HasColumnName("Content")
                .IsRequired();
        });
        builder.Property(x => x.Status)
            .HasConversion(
                x => x.Name, 
                x => Enumeration.GetByName<MessageStatus>(x));

        builder.HasOne(x => x.Chat);
        builder.HasOne(x => x.Sender);
    }
}