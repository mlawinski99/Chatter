using System.Linq.Expressions;
using Chatter.OutboxService;
using Chatter.Shared.DomainTypes;
using Chatter.Shared.Encryption.JsonSerializable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chatter.Shared.DataAccessTypes;

public abstract class BaseDbContext : DbContext, IUnitOfWork
{
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IEnumerable<IInterceptor> _interceptors;
    
    protected BaseDbContext(DbContextOptions options,
        IJsonSerializer jsonSerializer,
        IEnumerable<IInterceptor> interceptors)
        : base(options)
    {
        _jsonSerializer = jsonSerializer;
        _interceptors = interceptors;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_interceptors.Any())
            optionsBuilder.AddInterceptors(_interceptors);

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var filter = Expression.Lambda(Expression.Not(property), parameter);

                entityType.SetQueryFilter(filter);
            }
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (this is IOutbox)
        {
            await AddOutboxMessagesAsync(this as IOutbox, cancellationToken);
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
    private async Task AddOutboxMessagesAsync(IOutbox outboxContext, CancellationToken cancellationToken)
    {
        var domainEvents = ChangeTracker
            .Entries<AggregateRoot>()
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            var message = new OutboxMessage
            {
                OccurredOnUtc = domainEvent.OccurredOnUtc,
                Type = domainEvent.GetType().FullName!,
                Content = _jsonSerializer.Serialize(domainEvent),
                IsProcessed = false,
                ProcessedOn = null
            };

            outboxContext.OutboxMessages.Add(message);
        }

        foreach (var entry in ChangeTracker.Entries<AggregateRoot>())
            entry.Entity.ClearDomainEvents();

        await Task.CompletedTask;
    }
}