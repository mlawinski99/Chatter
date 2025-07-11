using Chatter.MessagesDataAccess.DbEntitiesConfigurations;
using Chatter.Shared.DataAccessTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chatter.MessagesDataAccess.DbContexts;

public class ChatDbContext : BaseDbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options, 
        IEnumerable<IInterceptor> interceptors) : base(options, interceptors)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ChatConfiguration());
        modelBuilder.ApplyConfiguration(new ChatMemberConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}