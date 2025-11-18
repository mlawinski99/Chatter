namespace Chatter.OutboxService;

public interface IOutboxMessageProcessor<TContext>
{
    Task ProcessAsync(CancellationToken cancellationToken);
}