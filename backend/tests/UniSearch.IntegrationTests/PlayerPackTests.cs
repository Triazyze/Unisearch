using System.Net;
using System.Net.Http.Json;
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
public class PlayerPackTests
{
    private readonly WebApplicationFactory<Program> _factory;

    public PlayerPackTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTestPack_AsPlayer_ReturnsExactlyTwoStoredGrids()
    {
        using var client = _factory.CreateClient();
        var grids = GetGridsCollection();
        var expectedGrids = await grids.Find(item => item.Pack!.Name == "test").ToListAsync();

        await AuthenticationHelper.AuthenticatePlayerAsync(client);

        var response = await client.GetAsync("api/packs/player/test");
        var pack = await response.Content.ReadFromJsonAsync<PlayerPackResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, expectedGrids.Count);
        Assert.NotNull(pack);
        Assert.Equal("test", pack.Pack.Name);
        Assert.Null(pack.Pack.Date);
        Assert.Equal(2, pack.Grids.Count);
        foreach (var expected in expectedGrids)
        {
            var grid = Assert.Single(pack.Grids, item => item.Id == expected.Id);
            Assert.Equal(expected.Size, grid.Size);
            Assert.Equal(expected.Letters, grid.Letters);
            Assert.Equal(expected.Words, grid.Words);
            Assert.Equal(expected.Difficulty, grid.Difficulty);
            Assert.Equal(expected.UpdatedAt, grid.UpdatedAt);
        }
    }

    [Fact]
    public async Task GetTestPack_WhenItContainsThreeGrids_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        var grids = GetGridsCollection();
        var extraGrid = new Grid
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Size = 4,
            Letters = ["CHAT", "XXXX", "XXXX", "XXXX"],
            Words = ["CHAT"],
            Difficulty = "Test",
            UpdatedAt = DateTime.UtcNow,
            Pack = new Grid.PackDetails { Name = "test", Date = null }
        };

        await AuthenticationHelper.AuthenticatePlayerAsync(client);
        await grids.InsertOneAsync(extraGrid);

        try
        {
            var response = await client.GetAsync("api/packs/player/test");
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(error);
            Assert.Equal(DomainApiErrors.TestPackNotFound.ErrorCode, error.ErrorCode);
            Assert.Equal(DomainApiErrors.TestPackNotFound.Message, error.Message);
        }
        finally
        {
            await grids.DeleteOneAsync(item => item.Id == extraGrid.Id);
        }
    }

    [Fact]
    public async Task GetTestPack_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("api/packs/player/test");
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal(GeneralApiErrors.Unauthorized.ErrorCode, error.ErrorCode);
    }

    [Fact]
    public async Task GetTestPack_AsAdmin_ReturnsForbidden()
    {
        using var client = _factory.CreateClient();
        await AuthenticationHelper.AuthenticateAdminAsync(client);

        var response = await client.GetAsync("api/packs/player/test");
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
