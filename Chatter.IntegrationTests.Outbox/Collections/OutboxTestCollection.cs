using Chatter.IntegrationTests.Outbox.Fixtures;
using Xunit;

namespace Chatter.IntegrationTests.Outbox.Collections;

[CollectionDefinition("Outbox")]
public class OutboxTestCollection : ICollectionFixture<OutboxTestFixture>;