using Microsoft.EntityFrameworkCore;

public class HangfireDbContext : DbContext
{
    public HangfireDbContext(DbContextOptions<HangfireDbContext> options)
        : base(options)
    {
    }
}