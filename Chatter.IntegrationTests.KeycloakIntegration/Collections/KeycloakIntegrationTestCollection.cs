using Chatter.IntegrationTests.KeycloakEventProcessor.Fixtures;
using Xunit;

namespace Chatter.IntegrationTests;

[CollectionDefinition("KeycloakIntegration")]
public class KeycloakIntegrationTestCollection : ICollectionFixture<KeycloakIntegrationTestFixture>
{
}
