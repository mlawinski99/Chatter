using Chatter.IntegrationTests.Migrator.Fixtures;
using Xunit;

namespace Chatter.IntegrationTests.Migrator.Collections;

[CollectionDefinition("Migrator")]
public class MigratorTestCollection : ICollectionFixture<MigratorTestFixture>;