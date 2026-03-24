using Chatter.IntegrationTests.Messages.Infrastructure;
using Xunit;

namespace Chatter.IntegrationTests.Messages.Collections;

[CollectionDefinition("MessagesApi")]
public class MessagesTestCollection : ICollectionFixture<MessagesTestFixture>
{
}