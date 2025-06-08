using Chatter.Shared.DomainTypes;
using Chatter.Shared.Encryption;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chatter.Shared.DataAccessTypes;

public class EncryptableInterceptor : SaveChangesInterceptor, IMaterializationInterceptor
{
    private readonly IEncryptor _encryptor;

    public EncryptableInterceptor(IEncryptor encryptor)
    {
        _encryptor = encryptor;
    }

    // Encrypt before save
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChanges(eventData, result);

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                EncryptEntity(entry.Entity);
            }
        }

        return base.SavingChanges(eventData, result);
    }

    // Decrypt after fetch
    public object InitializedInstance(MaterializationInterceptionData materializationData, object entity)
    {
        DecryptEntity(entity);
        return entity;
    }

    private void EncryptEntity(object entity)
    {
        var properties = entity.GetType().GetProperties()
            .Where(p => p.IsDefined(typeof(EncryptableAttribute), inherit: true)
                     && p.CanRead && p.CanWrite
                     && p.PropertyType == typeof(string));

        foreach (var prop in properties)
        {
            var plainText = (string)prop.GetValue(entity);
            if (!string.IsNullOrEmpty(plainText))
            {
                var encrypted = _encryptor.Encrypt(plainText);
                prop.SetValue(entity, encrypted);
            }
        }
    }

    private void DecryptEntity(object entity)
    {
        var properties = entity.GetType().GetProperties()
            .Where(p => p.IsDefined(typeof(EncryptableAttribute), inherit: true)
                     && p.CanRead && p.CanWrite
                     && p.PropertyType == typeof(string));

        foreach (var prop in properties)
        {
            var encrypted = (string)prop.GetValue(entity);
            if (!string.IsNullOrEmpty(encrypted))
            {
                var decrypted = _encryptor.Decrypt(encrypted);
                prop.SetValue(entity, decrypted);
            }
        }
    }
}
