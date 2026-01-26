using System.Text.Json;
using Chatter.Shared.DomainTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chatter.Shared.DataAccessTypes;

public class VersionableInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyVersioning(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyVersioning(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyVersioning(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker
            .Entries()
            .Where(e =>
                e.State == EntityState.Modified &&
                e.Entity is IVersionable)
            .ToList();

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            var originalId = (entity as Entity)?.Id;
            var cloned = CloneEntity(entity);

            entry.State = EntityState.Unchanged;

            if (cloned is Entity clonedEntity)
            {
                clonedEntity.Id = Guid.NewGuid();
            }

            if (cloned is IVersionable versionable)
            {
                versionable.VersionId += 1;
                versionable.VersionGroupId = versionable.VersionGroupId ?? originalId;
            }

            context.Add(cloned);
        }
    }

    private static object CloneEntity(object original)
    {
        var json = JsonSerializer.Serialize(original);
        return JsonSerializer.Deserialize(json, original.GetType())!;
    }
}