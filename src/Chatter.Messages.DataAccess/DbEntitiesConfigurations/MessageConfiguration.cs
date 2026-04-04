using Chatter.MessagesDomain;
using Chatter.Shared.DomainTypes;
using Core.DataAccessTypes;
using Core.DomainTypes;
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
            .ToTable("Messages", "chat");
        
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

        builder.HasOne(x => x.Chat)
            .WithMany()
            .HasForeignKey(x => x.ChatId);
        
        builder.HasOne(x => x.Sender)
            .WithMany()
            .HasForeignKey(x => x.SenderId);
    }
}