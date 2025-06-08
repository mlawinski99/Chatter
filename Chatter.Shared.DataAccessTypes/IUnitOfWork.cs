namespace Chatter.Shared.DataAccessTypes;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}