using Chatter.Shared.DataAccessTypes;
using Chatter.Shared.Logger;
using Chatter.Shared.KafkaProducer;
using Microsoft.EntityFrameworkCore;

namespace Chatter.OutboxService;

public class OutboxMessageProcessor<TContext> : IOutboxMessageProcessor<TContext> 
    where TContext : DbContext, IOutbox
{
    private readonly TContext _db;
    private readonly IAppLogger<OutboxMessageProcessor<TContext>> _logger;
    private readonly IProducer<OutboxMessage> _producer;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly int _batchSize = 100;
    
    public OutboxMessageProcessor(TContext db,
        IAppLogger<OutboxMessageProcessor<TContext>> logger,
        IProducer<OutboxMessage> producer,
        IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _logger = logger;
        _producer = producer;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _db.OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(_batchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var isProduceSucceded = _producer.Produce(message.Type, message);

                if (isProduceSucceded)
                {
                    message.ProcessedOn = _dateTimeProvider.UtcNow;
                    message.IsProcessed = true;
                    
                    await _db.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process outbox message {message.Id}", ex);
                _db.Entry(message).State = EntityState.Unchanged;
            }
        }

    }
}
