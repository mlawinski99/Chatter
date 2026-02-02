using Chatter.IntegrationTests.Kafka.Containers;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.Shared.Encryption.JsonSerializable;
using Chatter.Shared.KafkaConsumer;
using Chatter.Shared.KafkaProducer;
using Chatter.Shared.Logger;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Chatter.IntegrationTests.Kafka.Fixtures;

public class KafkaTestFixture : IAsyncLifetime
{
    private readonly KafkaContainerFixture _kafkaFixture = new();

    public string BootstrapServers => _kafkaFixture.BootstrapServers;

    public async Task InitializeAsync()
    {
        await _kafkaFixture.StartAsync();
    }

    public KafkaProducer<T> CreateProducer<T>(KafkaProducerConfiguration? config = null)
    {
        var configuration = config ?? new KafkaProducerConfiguration
        {
            BootstrapServers = BootstrapServers,
            EnableIdempotence = true,
            MessageTimeoutMs = 5000,
            Acks = "all"
        };

        var options = Options.Create(configuration);
        var logger = Substitute.For<IAppLogger<KafkaProducer<T>>>();
        var jsonSerializer = new TestJsonSerializer();

        return new KafkaProducer<T>(options, logger, jsonSerializer);
    }

    public KafkaConsumer CreateConsumer(KafkaConsumerConfiguration? config = null, List<string>? topics = null)
    {
        var configuration = config ?? new KafkaConsumerConfiguration
        {
            BootstrapServers = BootstrapServers,
            GroupId = $"test-group-{Guid.NewGuid()}",
            AllowedTopics = topics ?? new List<string> { "test-topic" },
            AutoOffsetReset = "earliest",
            EnableAutoCommit = true
        };

        var options = Options.Create(configuration);
        var logger = Substitute.For<IAppLogger<KafkaConsumer>>();

        return new KafkaConsumer(options, logger);
    }

    public IJsonSerializer CreateJsonSerializer() => new TestJsonSerializer();

    public async Task DisposeAsync()
    {
        await _kafkaFixture.DisposeAsync();
    }
}