using Chatter.Shared.DomainTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chatter.Shared.DataAccessTypes;

public class SoftDeletableInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public SoftDeletableInterceptor(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context == null)
            return new(result);

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                if (entry.Entity is ISoftDeletable softDeletable && 
                    entry.State == EntityState.Deleted)
                {
                    softDeletable.DateDeletedUtc = _dateTimeProvider.UtcNow;
                    softDeletable.IsDeleted = true;
                    entry.State = EntityState.Modified;
                }
            }
        }

        return new(result);
    }
}