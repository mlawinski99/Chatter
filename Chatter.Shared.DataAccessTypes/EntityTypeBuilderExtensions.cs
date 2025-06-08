using Chatter.Shared.DomainTypes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chatter.Shared.DataAccessTypes;

public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<TEntity> WithId<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity
    {
        builder.HasKey(e => e.Id);

        return builder;
    }
    
    public static EntityTypeBuilder<TEntity> WithAuditable<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity, IAuditable
    {
        builder.HasKey(e => e.DateCreatedUtc);
        builder.HasKey(e => e.DateModifiedUtc);

        return builder;
    }
    
    public static EntityTypeBuilder<TEntity> WithAuditableWithUser<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity, IAuditableWithUser
    {
        builder.HasKey(e => e.DateCreatedUtc);
        builder.HasKey(e => e.DateModifiedUtc);
        builder.HasKey(e => e.CreatedBy);
        builder.HasKey(e => e.ModifiedBy);

        return builder;
    }
    
    public static EntityTypeBuilder<TEntity> WithSoftDeletable<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity, ISoftDeletable
    {
        builder.Property(e => e.DateDeletedUtc);
        builder.Property(e => e.IsDeleted);

        return builder;
    }
}