using Chatter.Shared.Logger;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Chatter.Shared.KafkaConsumer;

public class KafkaConsumer : IConsumer
{
    private readonly IAppLogger<KafkaConsumer> _logger;
    private readonly HashSet<string> _allowedTopics;
    private readonly IConsumer<string, string> _consumer;

    public KafkaConsumer(IOptions<KafkaConsumerConfiguration> configuration,
        IAppLogger<KafkaConsumer> logger)
    {
        _logger = logger;
        var kafkaConfig = configuration.Value;
        _allowedTopics = new HashSet<string>(kafkaConfig.AllowedTopics);

        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaConfig.BootstrapServers,
            GroupId = kafkaConfig.GroupId,
            AutoOffsetReset = kafkaConfig.AutoOffsetReset.ToLower() == "latest"
                ? AutoOffsetReset.Latest
                : AutoOffsetReset.Earliest,
            EnableAutoCommit = kafkaConfig.EnableAutoCommit
        };
        _consumer = new ConsumerBuilder<string, string>(config).Build();

        _consumer.Subscribe(_allowedTopics.ToList());
    }

    public async Task StartAsync(
        Func<string, string, Task> handler,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = _consumer.Consume(cancellationToken);

            if (!_allowedTopics.Contains(result.Topic))
                continue;
            
            _logger.LogInformation("Received message with topic: {topic}, key: {key} and value: {msg}", result.Topic, result.Message.Key, result.Message);
            await handler(result.Topic, result.Message.Value);
        }
    }
}