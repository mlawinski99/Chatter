using Chatter.InfrastructureTests.Migrator.Fixtures;
using Xunit;

namespace Chatter.InfrastructureTests.Migrator.Collections;

[CollectionDefinition("Migrator")]
public class MigratorTestCollection : ICollectionFixture<MigratorTestFixture>;