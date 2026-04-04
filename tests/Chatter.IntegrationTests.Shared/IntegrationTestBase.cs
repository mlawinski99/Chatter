using Chatter.IntegrationTests.Shared.Fixtures;
using Chatter.IntegrationTests.Shared.Infrastructure;

namespace Chatter.IntegrationTests.Shared;

public abstract class IntegrationTestBase<TFixture> : IDisposable
    where TFixture : IntegrationTestFixtureBase
{
    protected TFixture Fixture { get; }
    protected TestDbContext Db { get; }

    protected IntegrationTestBase(TFixture fixture)
    {
        Fixture = fixture;
        Db = CreateDbContext();
    }

    protected virtual TestDbContext CreateDbContext() => Fixture.CreateDbContext();

    public virtual void Dispose()
    {
        Db.Dispose();
    }
}
