using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.Shared.Domain;
using Chatter.Users.DataAccess.DbContexts;

namespace Chatter.IntegrationTests.Users.Infrastructure;

public static class UsersDbSeeder
{
    public static void Seed(UsersDbContext db)
    {
        if (db.Users.Any(u => u.KeycloakId == Guid.Parse(KeycloakTestUsersData.TestUserId)))
            return;

        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            KeycloakId = Guid.Parse(KeycloakTestUsersData.TestUserId),
            UserName = KeycloakTestUsersData.TestUsername,
            Email = KeycloakTestUsersData.TestEmail
        });
        db.SaveChanges();
    }
}