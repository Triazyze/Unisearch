using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace UniSearch.IntegrationTests;

public class GridVerificationTests
{
    [Fact]
    public async Task Health_ShouldReturnOk()
    {
        using var factory = new WebApplicationFactory<Program>();

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
