using Chatter.IntegrationTests.Kafka.Fixtures;
using Xunit;

namespace Chatter.IntegrationTests.Kafka.Collections;

[CollectionDefinition("Kafka")]
public class KafkaTestCollection : ICollectionFixture<KafkaTestFixture>
{
}