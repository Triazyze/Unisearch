using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using UniSearch.Api.Errors;
using UniSearch.Api.Models.Dtos;
using UniSearch.Api.Models.Dtos.Grids;
using UniSearch.Api.Models.Entities;
using UniSearch.IntegrationTests.Helpers;

namespace UniSearch.IntegrationTests;

[Collection("IntegrationTests")]
public class AdminGridReadTests
{
    private readonly WebApplicationFactory<Program> _factory;

    public AdminGridReadTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllGrids_AsAdmin_ReturnsSummariesForStoredGrids()
    {
        using var client = _factory.CreateClient();
        var grids = GetGridsCollection();
        var draftGrid = new Grid
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Size = 4,
            Letters = ["CHAT", "XXXX", "XXXX", "XXXX"],
            Words = ["CHAT"],
            Difficulty = "Test",
            UpdatedAt = DateTime.UtcNow,
            Pack = null
        };

        await AuthenticationHelper.AuthenticateAdminAsync(client);
        await grids.InsertOneAsync(draftGrid);

        try
        {
            var expectedGrids = await grids.Find(FilterDefinition<Grid>.Empty).ToListAsync();
            var response = await client.GetAsync("api/grids");
            var summaries = await response.Content.ReadFromJsonAsync<List<AdminGridSummaryResponse>>();
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(summaries);
            Assert.Equal(expectedGrids.Count, summaries.Count);
            foreach (var expected in expectedGrids)
            {
                var summary = Assert.Single(summaries, item => item.Id == expected.Id);
                Assert.Equal(expected.Size, summary.Size);
                Assert.Equal(expected.Difficulty, summary.Difficulty);
                Assert.Equal(expected.Words.Count, summary.WordCount);
                Assert.Equal(expected.UpdatedAt, summary.UpdatedAt);
                Assert.Equal(expected.Pack?.Name, summary.Pack?.Name);
                Assert.Equal(expected.Pack?.Date, summary.Pack?.Date);
            }

            Assert.Null(Assert.Single(summaries, item => item.Id == draftGrid.Id).Pack);
            Assert.All(json.RootElement.EnumerateArray(), item =>
            {
                Assert.False(item.TryGetProperty("letters", out _));
                Assert.False(item.TryGetProperty("words", out _));
            });
        }
        finally
        {
            await grids.DeleteOneAsync(item => item.Id == draftGrid.Id);
        }
    }

    [Fact]
    public async Task GetGridById_AsAdmin_ReturnsStoredGridDetails()
    {
        using var client = _factory.CreateClient();
        var grids = GetGridsCollection();
        var expected = await grids.Find(item => item.Id == "66f000000000000000000001").FirstOrDefaultAsync();
        Assert.NotNull(expected);

        await AuthenticationHelper.AuthenticateAdminAsync(client);

        var response = await client.GetAsync($"api/grids/{expected.Id}");
        var details = await response.Content.ReadFromJsonAsync<AdminGridResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(details);
        Assert.Equal(expected.Id, details.Id);
        Assert.Equal(expected.Size, details.Size);
        Assert.Equal(expected.Letters, details.Letters);
        Assert.Equal(expected.Words, details.Words);
        Assert.Equal(expected.Difficulty, details.Difficulty);
        Assert.Equal(expected.UpdatedAt, details.UpdatedAt);
        Assert.Equal(expected.Pack?.Name, details.Pack?.Name);
        Assert.Equal(expected.Pack?.Date, details.Pack?.Date);
    }

    [Fact]
    public async Task GetGridById_WithInvalidId_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();
        await AuthenticationHelper.AuthenticateAdminAsync(client);

        var response = await client.GetAsync("api/grids/invalid-id");
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal(GeneralApiErrors.BadRequest.ErrorCode, error.ErrorCode);
    }

    [Fact]
    public async Task GetGridById_WithUnknownId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        await AuthenticationHelper.AuthenticateAdminAsync(client);

        var response = await client.GetAsync("api/grids/66f000000000000000000003");
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal(DomainApiErrors.GridNotFound.ErrorCode, error.ErrorCode);
        Assert.Equal(DomainApiErrors.GridNotFound.Message, error.Message);
    }

    [Theory]
    [InlineData("api/grids")]
    [InlineData("api/grids/66f000000000000000000001")]
    public async Task GetAdminGrid_WithoutAuthentication_ReturnsUnauthorized(string route)
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(route);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal(GeneralApiErrors.Unauthorized.ErrorCode, error.ErrorCode);
    }

    [Theory]
    [InlineData("api/grids")]
    [InlineData("api/grids/66f000000000000000000001")]
    public async Task GetAdminGrid_AsPlayer_ReturnsForbidden(string route)
    {
        using var client = _factory.CreateClient();
        await AuthenticationHelper.AuthenticatePlayerAsync(client);

        var response = await client.GetAsync(route);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal(GeneralApiErrors.Forbidden.ErrorCode, error.ErrorCode);
    }

    private IMongoCollection<Grid> GetGridsCollection()
    {
        var database = _factory.Services.GetRequiredService<IMongoDatabase>();
        return database.GetCollection<Grid>("grids");
    }
}
