using Microsoft.EntityFrameworkCore;

namespace Chatter.OutboxService;

public interface IOutbox
{
    DbSet<OutboxMessage> OutboxMessages { get; set; }
}