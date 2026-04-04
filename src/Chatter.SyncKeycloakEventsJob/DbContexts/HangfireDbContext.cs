using Microsoft.EntityFrameworkCore;

namespace Chatter.SyncKeycloakEvents.DbContexts;

public class HangfireDbContext : DbContext
{
    public HangfireDbContext(DbContextOptions<HangfireDbContext> options)
        : base(options)
    {
    }
}