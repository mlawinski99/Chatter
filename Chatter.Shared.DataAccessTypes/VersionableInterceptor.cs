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
        var context = eventData.Context;
        if (context == null) return base.SavingChanges(eventData, result);

        var entries = context.ChangeTracker
            .Entries()
            .Where(e =>
                e.State == EntityState.Modified &&
                e.Entity is IVersionable)
            .ToList();

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            entry.State = EntityState.Unchanged;
            
            var cloned = CloneEntity(entity);

            if (cloned is IVersionable)
            {
                var versionable = (IVersionable)cloned;
                versionable.VersionId += 1;
                versionable.VersionGroupId = versionable.VersionGroupId == null ?
                    (versionable as Entity).Id :
                    versionable.VersionGroupId;
            }

            context.Add(cloned);
        }

        return base.SavingChanges(eventData, result);
    }

    private static object CloneEntity(object original)
    {
        // @TODO - skip ID during serialization?
        var json = JsonSerializer.Serialize(original);
        return JsonSerializer.Deserialize(json, original.GetType())!;
    }
}