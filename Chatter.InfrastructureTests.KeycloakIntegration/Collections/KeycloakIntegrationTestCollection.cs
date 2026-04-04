using Chatter.IntegrationTests.Shared.Fixtures;
using Xunit;

namespace Chatter.InfrastructureTests.KeycloakIntegration;

[CollectionDefinition("KeycloakIntegration")]
public class KeycloakIntegrationTestCollection : ICollectionFixture<KeycloakIntegrationTestFixture>
{
}
