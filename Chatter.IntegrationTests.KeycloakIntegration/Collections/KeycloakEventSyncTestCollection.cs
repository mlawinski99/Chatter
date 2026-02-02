using Chatter.IntegrationTests.KeycloakEventProcessor.Fixtures;
using Xunit;

namespace Chatter.IntegrationTests;

[CollectionDefinition("KeycloakEventSync")]
public class KeycloakEventSyncTestCollection : ICollectionFixture<KeycloakEventSyncTestFixture>
{
}
