using Chatter.InfrastructureTests.Outbox.Fixtures;
using Xunit;

namespace Chatter.InfrastructureTests.Outbox.Collections;

[CollectionDefinition("Outbox")]
public class OutboxTestCollection : ICollectionFixture<OutboxTestFixture>;