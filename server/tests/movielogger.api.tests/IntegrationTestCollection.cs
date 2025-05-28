using movielogger.api.tests.fixtures;
using Xunit;

namespace movielogger.api.tests;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{

}