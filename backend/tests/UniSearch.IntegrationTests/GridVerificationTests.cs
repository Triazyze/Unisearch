using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;
using UniSearch.Api.Errors;
using UniSearch.Api.Models.Dtos;
using UniSearch.Api.Models.Dtos.Authentication;
using UniSearch.Api.Models.Dtos.Grids;
using UniSearch.Api.Models.Entities;
using UniSearch.IntegrationTests.Helpers;

namespace UniSearch.IntegrationTests;

[Collection("IntegrationTests")]
public class GridVerificationTests
{
    private readonly WebApplicationFactory<Program> _factory;

    public GridVerificationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    // Méthode pour créer des cas invalides de vérification de grille
    public static TheoryData<Action<GridVerificationRequest>, ErrorResponse> InvalidGridCases => new ()
    {
        { grid => grid.Letters[0] = "XXXX", DomainApiErrors.LettersInvalid },
        { grid => grid.Words[0] = "CHIEN", DomainApiErrors.WordsInvalid },
        { grid => grid.Traces[0].Word = "CHIEN", DomainApiErrors.TraceMismatch },
        { grid => grid.Traces[0].StartRow = -1, DomainApiErrors.TracesInvalid },
        { grid => grid.Traces[0].StartColumn = -1, DomainApiErrors.TracesInvalid },
        { grid => grid.Traces[0].EndRow = -1, DomainApiErrors.TracesInvalid },
        { grid => grid.Traces[0].EndColumn = -1, DomainApiErrors.TracesInvalid },
        { grid => grid.Traces.Clear(), DomainApiErrors.TraceMismatch },
        { grid => grid.Traces.Add(grid.Traces[0]), DomainApiErrors.TraceMismatch },
        {
            grid =>
            {
                grid.Traces[0].StartRow = 0;
                grid.Traces[0].EndRow = 1;
            },
            DomainApiErrors.TracesInvalid
        }
    };

    // Méthode pour créer une requête de vérification de grille valide
    private static GridVerificationRequest CreateValidGridRequest()
    {
        return new GridVerificationRequest
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
        };
    }

    [Fact]
    public async Task VerifyGrid_WithValidData_ReturnsOk()
    {
        using var client = _factory.CreateClient();
        var database = _factory.Services.GetRequiredService<IMongoDatabase>();

        var playerResultsCollection = database.GetCollection<PlayerResult>("playerResults");
        var usersCollection = database.GetCollection<User>("users");
        var grid = CreateValidGridRequest();
        var gridId = "66f000000000000000000001";

        var player = await usersCollection
            .Find(u => u.Username == "player")
            .FirstOrDefaultAsync();
        Assert.NotNull(player);

        await playerResultsCollection.DeleteManyAsync(r => r.UserId == player.Id && r.GridId == gridId);

        await AuthenticationHelper.AuthenticatePlayerAsync(client);

        var response = await client.PostAsJsonAsync($"api/grids/{gridId}/verification", grid);

        var result = await playerResultsCollection
            .Find(r => r.UserId == player.Id && r.GridId == gridId)
            .FirstOrDefaultAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(player.Id, result.UserId);
        Assert.Equal(gridId, result.GridId);
        Assert.Equal(10, result.Time);
        Assert.Equal(3, result.TraceAttempts);
    }

    [Fact]
    public async Task VerifyGrid_WithExistingResult_UpdatesPlayerResult()
    {
        using var client = _factory.CreateClient();
        var database = _factory.Services.GetRequiredService<IMongoDatabase>();

        var playerResultsCollection = database.GetCollection<PlayerResult>("playerResults");
        var usersCollection = database.GetCollection<User>("users");
        var grid = CreateValidGridRequest();
        var gridId = "66f000000000000000000001";

        var player = await usersCollection
            .Find(u => u.Username == "player")
            .FirstOrDefaultAsync();
        Assert.NotNull(player);

        // Nettoyer les anciens résultats pour garantir l'indépendance du test
        await playerResultsCollection.DeleteManyAsync(
            r => r.UserId == player.Id && r.GridId == gridId);

        await AuthenticationHelper.AuthenticatePlayerAsync(client);

        // Première soumission : création du résultat
        var firstResponse = await client.PostAsJsonAsync(
            $"api/grids/{gridId}/verification", grid);

        var firstResult = await playerResultsCollection
            .Find(r => r.UserId == player.Id && r.GridId == gridId)
            .FirstOrDefaultAsync();

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.NotNull(firstResult);

        // Deuxième soumission : modification des valeurs
        grid.Time = 20;
        grid.TraceAttempts = 5;

        var secondResponse = await client.PostAsJsonAsync(
            $"api/grids/{gridId}/verification", grid);

        var results = await playerResultsCollection
            .Find(r => r.UserId == player.Id && r.GridId == gridId)
            .ToListAsync();

        // Vérifier que le résultat existant a été mis à jour
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        Assert.Single(results);
        Assert.Equal(firstResult.Id, results[0].Id);
        Assert.Equal(20, results[0].Time);
        Assert.Equal(5, results[0].TraceAttempts);
    }

    [Theory]
    [MemberData(nameof(InvalidGridCases))]
    public async Task VerifyGrid_WithInvalidData_ReturnsUnprocessableEntity(
        Action<GridVerificationRequest> modifyGrid,
        ErrorResponse expectedError)
    {
        using var client = _factory.CreateClient();
        var database = _factory.Services.GetRequiredService<IMongoDatabase>();

        var playerResultsCollection = database.GetCollection<PlayerResult>("playerResults");
        var usersCollection = database.GetCollection<User>("users");
        var grid = CreateValidGridRequest();
        modifyGrid(grid);
        var gridId = "66f000000000000000000001";

        var player = await usersCollection
            .Find(u => u.Username == "player")
            .FirstOrDefaultAsync();
        Assert.NotNull(player);

        await playerResultsCollection.DeleteManyAsync(r => r.UserId == player.Id && r.GridId == gridId);

        await AuthenticationHelper.AuthenticatePlayerAsync(client);

        var response = await client.PostAsJsonAsync($"api/grids/{gridId}/verification", grid);

        var result = await playerResultsCollection
            .Find(r => r.UserId == player.Id && r.GridId == gridId)
            .FirstOrDefaultAsync();
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal(expectedError.Message, error.Message);
        Assert.Null(result);
    }

    [Theory]
    [MemberData(nameof(InvalidGridCases))]
    public async Task VerifyGrid_WithInvalidData_PreservesExistingPlayerResult(
        Action<GridVerificationRequest> modifyGrid,
        ErrorResponse expectedError)
    {
        using var client = _factory.CreateClient();
        var database = _factory.Services.GetRequiredService<IMongoDatabase>();
        var playerResultsCollection = database.GetCollection<PlayerResult>("playerResults");
        var usersCollection = database.GetCollection<User>("users");
        var player = await usersCollection.Find(user => user.Username == "player").FirstOrDefaultAsync();
        Assert.NotNull(player);

        var gridId = "66f000000000000000000001";
        var existingResult = new PlayerResult
        {
            Id = ObjectId.GenerateNewId().ToString(),
            UserId = player.Id,
            GridId = gridId,
            Time = 12,
            TraceAttempts = 4
        };

        await playerResultsCollection.DeleteManyAsync(result => result.UserId == player.Id && result.GridId == gridId);
        await playerResultsCollection.InsertOneAsync(existingResult);

        try
        {
            await AuthenticationHelper.AuthenticatePlayerAsync(client);
            var grid = CreateValidGridRequest();
            modifyGrid(grid);

            var response = await client.PostAsJsonAsync($"api/grids/{gridId}/verification", grid);
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            var results = await playerResultsCollection
                .Find(result => result.UserId == player.Id && result.GridId == gridId)
                .ToListAsync();

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.NotNull(error);
            Assert.Equal(expectedError.ErrorCode, error.ErrorCode);
            Assert.Equal(expectedError.Message, error.Message);
            var persistedResult = Assert.Single(results);
            Assert.Equal(existingResult.Id, persistedResult.Id);
            Assert.Equal(existingResult.UserId, persistedResult.UserId);
            Assert.Equal(existingResult.GridId, persistedResult.GridId);
            Assert.Equal(existingResult.Time, persistedResult.Time);
            Assert.Equal(existingResult.TraceAttempts, persistedResult.TraceAttempts);
        }
        finally
        {
            await playerResultsCollection.DeleteManyAsync(result => result.UserId == player.Id && result.GridId == gridId);
        }
    }

    [Fact]
    public async Task VerifyGrid_WithWrongRole_ReturnsForbidden()
    {
        using var client = _factory.CreateClient();
        var database = _factory.Services.GetRequiredService<IMongoDatabase>();

        var playerResultsCollection = database.GetCollection<PlayerResult>("playerResults");
        var usersCollection = database.GetCollection<User>("users");
        var grid = CreateValidGridRequest();
        var gridId = "66f000000000000000000001";

        var admin = await usersCollection
            .Find(u => u.Username == "admin")
            .FirstOrDefaultAsync();
        Assert.NotNull(admin);

        await AuthenticationHelper.AuthenticateAdminAsync(client);

        var response = await client.PostAsJsonAsync($"api/grids/{gridId}/verification", grid);

        var result = await playerResultsCollection
            .Find(r => r.UserId == admin.Id && r.GridId == gridId)
            .FirstOrDefaultAsync();

        Assert.Null(result);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task VerifyGrid_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        var database = _factory.Services.GetRequiredService<IMongoDatabase>();

        var playerResultsCollection = database.GetCollection<PlayerResult>("playerResults");
        var grid = CreateValidGridRequest();
        var gridId = "66f000000000000000000001";

        var countBefore = await playerResultsCollection.CountDocumentsAsync(FilterDefinition<PlayerResult>.Empty);

        var response = await client.PostAsJsonAsync($"api/grids/{gridId}/verification", grid);
        var countAfter = await playerResultsCollection.CountDocumentsAsync(FilterDefinition<PlayerResult>.Empty);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(countBefore, countAfter);
    }

    [Fact]
    public async Task VerifyGrid_WhenGridIsNotFound_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();
        var database = _factory.Services.GetRequiredService<IMongoDatabase>();

        var playerResultsCollection = database.GetCollection<PlayerResult>("playerResults");
        var usersCollection = database.GetCollection<User>("users");
        var grid = CreateValidGridRequest();
        var gridId = "66f000000000000000000003";

        var player = await usersCollection
            .Find(u => u.Username == "player")
            .FirstOrDefaultAsync();
        Assert.NotNull(player);

        await AuthenticationHelper.AuthenticatePlayerAsync(client);

        var response = await client.PostAsJsonAsync($"api/grids/{gridId}/verification", grid);

        var result = await playerResultsCollection
            .Find(r => r.UserId == player.Id && r.GridId == gridId)
            .FirstOrDefaultAsync();

        Assert.Null(result);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("invalid-id", false)]
    [InlineData("66f000000000000000000001", true)]
    public async Task VerifyGrid_WithInvalidRequest_ReturnsBadRequest(
        string gridId,
        bool invalidBody)
    {
        using var client = _factory.CreateClient();
        var database = _factory.Services.GetRequiredService<IMongoDatabase>();

        var playerResultsCollection = database.GetCollection<PlayerResult>("playerResults");

        object grid = invalidBody
            ? "This is not a valid request"
            : CreateValidGridRequest();

        await AuthenticationHelper.AuthenticatePlayerAsync(client);

        var countBefore = await playerResultsCollection.CountDocumentsAsync(FilterDefinition<PlayerResult>.Empty);

        var response = await client.PostAsJsonAsync($"api/grids/{gridId}/verification", grid);

        var countAfter = await playerResultsCollection.CountDocumentsAsync(FilterDefinition<PlayerResult>.Empty);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(countBefore, countAfter);
    }
}
