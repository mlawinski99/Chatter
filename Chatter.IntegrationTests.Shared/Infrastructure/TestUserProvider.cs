using Chatter.Shared.DataAccessTypes;

namespace Chatter.IntegrationTests.Shared.Infrastructure;

public class TestUserProvider : IUserProvider
{
    public Guid? UserId { get; set; }
}
