using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;
using UniSearch.Api.Models.Dtos.Authentication;
using UniSearch.Api.Models.Dtos.Grids;
using UniSearch.Api.Models.Entities;

namespace UniSearch.IntegrationTests;

[Collection("IntegrationTests")]
public class GridVerificationTests
{
    private readonly WebApplicationFactory<Program> _factory;

    public GridVerificationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task VerifiyGrid_WithValidData_ReturnsOk()
    {
        using var client = _factory.CreateClient();
        var database = _factory.Services.GetRequiredService<IMongoDatabase>();
        var playerResultsCollection = database.GetCollection<PlayerResult>("playerResults");
        var usersCollection = database.GetCollection<User>("users");

        var password = Environment.GetEnvironmentVariable("SEED_PLAYER_PASSWORD") ?? throw new InvalidOperationException("SEED_PLAYER_PASSWORD n'est pas renseigné.");
        var loginRequest = await client.PostAsJsonAsync("api/auth/login", new LoginRequest { Identifier = "player", Password = password });
        Assert.Equal(HttpStatusCode.OK, loginRequest.StatusCode);

        var loginResponse = await loginRequest.Content.ReadFromJsonAsync<LoginResponse>() ?? throw new InvalidOperationException("Impossible de récupérer le token d'authentification.");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);

        var response = await client.PostAsJsonAsync("api/grids/66f000000000000000000001/verification", new GridVerificationRequest
        {
            Letters = new List<string>
            {
            "CHAT",
            "XXXX",
            "XXXX",
            "XXXX",
            },
            Words = new List<string> { "CHAT" },
            Time = 10,
            TraceAttempts = 3,
            Traces = new List<Trace>
            {
                new Trace
                {
                    Word = "CHAT",
                    StartRow = 0,
                    StartColumn = 0,
                    EndRow = 0,
                    EndColumn = 3
                }
            }
        });

        var player = await usersCollection
            .Find(u => u.Username == "player")
            .FirstOrDefaultAsync();

        var result = await playerResultsCollection
            .Find(r => r.UserId == player.Id && r.GridId == "66f000000000000000000001")
            .FirstOrDefaultAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.True(ObjectId.TryParse(result.Id, out _));
        Assert.Equal(player.Id, result.UserId);
        Assert.Equal("66f000000000000000000001", result.GridId);
        Assert.Equal(10, result.Time);
        Assert.Equal(3, result.TraceAttempts);
    }
}
