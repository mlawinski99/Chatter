namespace Core.KafkaProducer;

public interface IProducer<T>
{
    Task<bool> ProduceAsync(string topic, T message, CancellationToken cancellationToken = default);
}