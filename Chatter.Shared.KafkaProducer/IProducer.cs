namespace Chatter.Shared.KafkaProducer;

public interface IProducer<T>
{
    bool Produce(string topic, T message);
}