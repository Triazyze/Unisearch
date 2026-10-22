using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using UniSearch.Api.Errors;
using UniSearch.Api.Models.Dtos;
using UniSearch.Api.Models.Dtos.Grids;
using UniSearch.Api.Models.Entities;
using UniSearch.IntegrationTests.Helpers;

namespace UniSearch.IntegrationTests;

[Collection("IntegrationTests")]
public class GridCreationTests
{
    private static readonly (int Row, int Column)[] Directions =
    [(-1, 0), (-1, 1), (0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1)];

    private readonly WebApplicationFactory<Program> _factory;

    public GridCreationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public static TheoryData<GridCreationRequest, ErrorResponse> InvalidGridRequests => new ()
    {
        { new GridCreationRequest { Size = 3, Difficulty = "easy", Words = ["CHAT"] }, DomainApiErrors.IncorrectGridSize },
        { new GridCreationRequest { Size = 11, Difficulty = "easy", Words = ["CHAT"] }, DomainApiErrors.IncorrectGridSize },
        { new GridCreationRequest { Size = 4, Difficulty = "easy", Words = [] }, DomainApiErrors.NotEnoughWords },
        { new GridCreationRequest { Size = 4, Difficulty = "easy", Words = ["CHAT", "CHAT"] }, DomainApiErrors.DuplicateWords },
        { new GridCreationRequest { Size = 4, Difficulty = "easy", Words = ["KAYAK"] }, DomainApiErrors.InvalidWords },
        { new GridCreationRequest { Size = 4, Difficulty = "easy", Words = ["AB"] }, DomainApiErrors.InvalidWords },
        { new GridCreationRequest { Size = 4, Difficulty = "easy", Words = ["CHIEN"] }, DomainApiErrors.PlacementFailed },
        { new GridCreationRequest { Size = 4, Difficulty = string.Empty, Words = ["CHAT"] }, DomainApiErrors.MissingDifficulty },
    };

    private static List<string> CreateManyValidWords()
    {
        var words = new List<string>();

        for (var first = 'A'; first <= 'Z' && words.Count < 50; first++)
        {
            for (var second = 'A'; second <= 'Z' && words.Count < 50; second++)
            {
                if (first != second)
                {
                    words.Add($"{first}{second}Z");
                }
            }
        }

        return words;
    }

    private static int CountWordOccurrences(List<string> letters, string word)
    {
        var count = 0;

        for (var row = 0; row < letters.Count; row++)
        {
            for (var column = 0; column < letters[row].Length; column++)
            {
                foreach (var direction in Directions)
                {
                    if (DoesWordMatchAt(letters, word, row, column, direction.Row, direction.Column))
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    private static bool DoesWordMatchAt(
        List<string> letters,
        string word,
        int startRow,
        int startColumn,
        int rowDirection,
        int columnDirection)
    {
        for (var index = 0; index < word.Length; index++)
        {
            var row = startRow + (index * rowDirection);
            var column = startColumn + (index * columnDirection);

            if (row < 0 || row >= letters.Count || column < 0 || column >= letters[row].Length || letters[row][column] != word[index])
            {
                return false;
            }
        }

        return true;
    }

    [Fact]
    public async Task CreateGrid_WithValidData_ReturnsCreatedAndPersistsCompleteDraftGrid()
    {
        using var client = _factory.CreateClient();
        var gridsCollection = GetGridsCollection();
        var difficulty = $"integration-{Guid.NewGuid()}";
        var request = new GridCreationRequest
        {
            Size = 4,
            Difficulty = difficulty,
            Words = ["CHAT", "LION"]
        };

        await AuthenticationHelper.AuthenticateAdminAsync(client);

        var response = await client.PostAsJsonAsync("api/grids", request);
        var grid = await gridsCollection.Find(item => item.Difficulty == difficulty).FirstOrDefaultAsync();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(grid);
        Assert.Equal(request.Size, grid.Size);
        Assert.Equal(request.Difficulty, grid.Difficulty);
        Assert.Equal(request.Words, grid.Words);
        Assert.Null(grid.Pack);
        Assert.Equal(request.Size, grid.Letters.Count);
        Assert.All(grid.Letters, row =>
        {
            Assert.Equal(request.Size, row.Length);
            Assert.All(row, character => Assert.InRange(character, 'A', 'Z'));
        });
        Assert.All(request.Words, word => Assert.Equal(1, CountWordOccurrences(grid.Letters, word)));

        await gridsCollection.DeleteOneAsync(item => item.Id == grid.Id);
    }

    [Theory]
    [MemberData(nameof(InvalidGridRequests))]
    public async Task CreateGrid_WithInvalidData_ReturnsExpectedUnprocessableEntity(
        GridCreationRequest request,
        ErrorResponse expectedError)
    {
        using var client = _factory.CreateClient();
        var gridsCollection = GetGridsCollection();

        await AuthenticationHelper.AuthenticateAdminAsync(client);

        var countBefore = await gridsCollection.CountDocumentsAsync(FilterDefinition<Grid>.Empty);
        var response = await client.PostAsJsonAsync("api/grids", request);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        var countAfter = await gridsCollection.CountDocumentsAsync(FilterDefinition<Grid>.Empty);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal(expectedError.Message, error.Message);
        Assert.Equal(countBefore, countAfter);
    }

    [Fact]
    public async Task CreateGrid_WhenTotalWordLengthExceedsGridCapacity_ReturnsUnprocessableEntity()
    {
        using var client = _factory.CreateClient();
        var gridsCollection = GetGridsCollection();
        var request = new GridCreationRequest
        {
            Size = 4,
            Difficulty = "easy",
            Words = CreateManyValidWords()
        };

        await AuthenticationHelper.AuthenticateAdminAsync(client);

        var countBefore = await gridsCollection.CountDocumentsAsync(FilterDefinition<Grid>.Empty);
        var response = await client.PostAsJsonAsync("api/grids", request);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        var countAfter = await gridsCollection.CountDocumentsAsync(FilterDefinition<Grid>.Empty);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal(DomainApiErrors.PlacementFailed.Message, error.Message);
        Assert.Equal(countBefore, countAfter);
    }

    [Fact]
    public async Task CreateGrid_WithMalformedRequest_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();
        var gridsCollection = GetGridsCollection();

        await AuthenticationHelper.AuthenticateAdminAsync(client);

        var countBefore = await gridsCollection.CountDocumentsAsync(FilterDefinition<Grid>.Empty);
        using var content = new StringContent("{invalid-json", System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("api/grids", content);
        var countAfter = await gridsCollection.CountDocumentsAsync(FilterDefinition<Grid>.Empty);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(countBefore, countAfter);
    }

    [Fact]
    public async Task CreateGrid_WithPlayerRole_ReturnsForbidden()
    {
        using var client = _factory.CreateClient();
        var gridsCollection = GetGridsCollection();
        var request = new GridCreationRequest { Size = 4, Difficulty = "easy", Words = ["CHAT"] };

        await AuthenticationHelper.AuthenticatePlayerAsync(client);

        var countBefore = await gridsCollection.CountDocumentsAsync(FilterDefinition<Grid>.Empty);
        var response = await client.PostAsJsonAsync("api/grids", request);
        var countAfter = await gridsCollection.CountDocumentsAsync(FilterDefinition<Grid>.Empty);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(countBefore, countAfter);
    }

    [Fact]
    public async Task CreateGrid_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        var request = new GridCreationRequest { Size = 4, Difficulty = "easy", Words = ["CHAT"] };

        var response = await client.PostAsJsonAsync("api/grids", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private IMongoCollection<Grid> GetGridsCollection()
    {
        var database = _factory.Services.GetRequiredService<IMongoDatabase>();
        return database.GetCollection<Grid>("grids");
    }
}
