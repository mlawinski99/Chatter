using Chatter.IntegrationTests.Users.Infrastructure;
using Xunit;

namespace Chatter.IntegrationTests.Users.Collections;

[CollectionDefinition("UsersApi")]
public class UsersTestCollection : ICollectionFixture<UsersTestFixture>
{
}