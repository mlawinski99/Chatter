using Chatter.Shared.Encryption.JsonSerializable;
using Chatter.Shared.Logger;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Chatter.Shared.KafkaProducer;

public class KafkaProducer<T> : IProducer<T>, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly IAppLogger<KafkaProducer<T>> _logger;
    private readonly IJsonSerializer _jsonSerializer;

    public KafkaProducer(IOptions<KafkaProducerConfiguration> configuration,
        IAppLogger<KafkaProducer<T>> logger,
        IJsonSerializer jsonSerializer)
    {
        var kafkaConfig = configuration.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = kafkaConfig.BootstrapServers,
            EnableIdempotence = kafkaConfig.EnableIdempotence,
            MessageTimeoutMs = kafkaConfig.MessageTimeoutMs,
            Acks = kafkaConfig.Acks.ToLowerInvariant() switch
            {
                "none"   => Acks.None,
                "leader" => Acks.Leader,
                _        => Acks.All
            }
        };
        
        _producer = new ProducerBuilder<string, string>(config).Build();
        _logger = logger;
        _jsonSerializer = jsonSerializer;
    }

    public bool Produce(string topic, T message)
    {
        var serializedMessage = _jsonSerializer.Serialize(message);
        var id = Guid.NewGuid().ToString();
        var result = false;
        _producer.Produce(topic, new Message<string, string>
            {
                Key = id,
                Value = serializedMessage
            },
            deliveryReport =>
            {
                if (deliveryReport.Error.IsError)
                {
                    _logger.LogError("Failed on delivery message {Id}: {Reason}", id, deliveryReport.Error.Reason);
                    result = false;
                }
                else
                {
                    _logger.LogInformation("Successfully delivered message {Id}", id);
                    result = true;
                }
            });
        
        return result;
    }

    public void Dispose() => _producer.Flush(TimeSpan.FromSeconds(5));
}