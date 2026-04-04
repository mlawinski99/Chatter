using Chatter.InfrastructureTests.Kafka.Fixtures;
using Xunit;

namespace Chatter.InfrastructureTests.Kafka.Collections;

[CollectionDefinition("Kafka")]
public class KafkaTestCollection : ICollectionFixture<KafkaTestFixture>
{
}