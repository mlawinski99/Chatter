using Chatter.InfrastructureTests.KeycloakIntegration.Fixtures;
using Xunit;

namespace Chatter.InfrastructureTests.KeycloakIntegration;

[CollectionDefinition("KeycloakEventSync")]
public class KeycloakEventSyncTestCollection : ICollectionFixture<KeycloakEventSyncTestFixture>
{
}
