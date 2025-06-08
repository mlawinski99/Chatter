using Chatter.Shared.DomainTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chatter.Shared.DataAccessTypes;

public class AuditableInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _clock;
    private readonly IUserProvider _user;

    public AuditableInterceptor(IDateTimeProvider clock, IUserProvider user)
    {
        _clock = clock;
        _user = user;
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
                if (entry.Entity is IAuditableWithUser auditableWithUser)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditableWithUser.CreatedBy = _user.UserId;
                        auditableWithUser.DateCreatedUtc = _clock.UtcNow;
                    }

                    auditableWithUser.ModifiedBy = _user.UserId;
                    auditableWithUser.DateModifiedUtc = _clock.UtcNow;
                }
                else if (entry.Entity is IAuditable auditable)
                {
                    if (entry.State == EntityState.Added)
                        auditable.DateCreatedUtc = _clock.UtcNow;
                    auditable.DateModifiedUtc = _clock.UtcNow;
                }
            }
        }

        return new(result);
    }
}