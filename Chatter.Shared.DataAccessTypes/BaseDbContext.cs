using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chatter.Shared.DataAccessTypes;

public abstract class BaseDbContext : DbContext, IUnitOfWork
{
    private readonly IEnumerable<IInterceptor> _interceptors;

    protected BaseDbContext(DbContextOptions options, IEnumerable<IInterceptor> interceptors)
        : base(options)
    {
        _interceptors = interceptors;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_interceptors.Any())
            optionsBuilder.AddInterceptors(_interceptors);

        base.OnConfiguring(optionsBuilder);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}