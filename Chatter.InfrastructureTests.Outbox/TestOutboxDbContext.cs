using Chatter.OutboxService;
using Microsoft.EntityFrameworkCore;

namespace Chatter.InfrastructureTests.Outbox;

public class TestOutboxDbContext : DbContext, IOutbox
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public TestOutboxDbContext(DbContextOptions<TestOutboxDbContext> options)
        : base(options)
    {
    }
}