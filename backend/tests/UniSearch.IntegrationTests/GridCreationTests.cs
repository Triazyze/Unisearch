using System.Net;

namespace UniSearch.IntegrationTests;

[Collection("IntegrationTests")]
public class GridCreationTests
{
    private readonly WebApplicationFactory<Program> _factory;

    public GridCreationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateGrid_WhenGridIsCreated_ReturnsOk()
    {
        using var client = _factory.CreateClient();
    }
}
