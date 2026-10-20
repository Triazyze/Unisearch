namespace UniSearch.IntegrationTests;

// Cette classe est utilisée pour créer une collection de tests, pour partager la même instance de WAF.
[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<WebApplicationFactory<Program>>
{
}
