using Chatter.MessagesDataAccess.DbEntitiesConfigurations;
using Chatter.MessagesDomain;
using Chatter.Shared.DataAccessTypes;
using Chatter.Shared.Encryption.JsonSerializable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chatter.MessagesDataAccess.DbContexts;

public class ChatDbContext : BaseDbContext
{
    public DbSet<Message> Messages { get; set; }
    public DbSet<ChatMember> ChatMembers { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<KeycloakAdminEvent> KeycloakAdminEvents { get; set; }
    public ChatDbContext(DbContextOptions<ChatDbContext> options, 
        IJsonSerializer jsonSerializer,
        IEnumerable<IInterceptor> interceptors) : base(options, jsonSerializer, interceptors)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ChatConfiguration());
        modelBuilder.ApplyConfiguration(new ChatMemberConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new KeycloakAdminEventConfiguration());
    }
}