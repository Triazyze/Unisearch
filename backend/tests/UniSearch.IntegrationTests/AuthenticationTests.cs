using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using UniSearch.Api.Errors;
using UniSearch.Api.Models.Dtos;
using UniSearch.Api.Models.Dtos.Authentication;
using UniSearch.Api.Models.Entities;

namespace UniSearch.IntegrationTests;

[Collection("IntegrationTests")]
public class AuthenticationTests
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthenticationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_WithValidData_CreatesPlayerWithHashedPassword()
    {
        using var client = _factory.CreateClient();
        var users = GetUsersCollection();
        var username = $"player-{Guid.NewGuid():N}";
        var request = new RegisterRequest
        {
            Username = username,
            Email = $"{username}@example.com",
            Password = "ValidPassword1!"
        };

        try
        {
            var response = await client.PostAsJsonAsync("api/auth/register", request);
            var user = await users.Find(item => item.Username == username).FirstOrDefaultAsync();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(user);
            Assert.Equal(request.Email, user.Email);
            Assert.Equal(Role.Player, user.Role);
            Assert.NotEqual(request.Password, user.PasswordHash);
            Assert.False(string.IsNullOrWhiteSpace(user.PasswordHash));
        }
        finally
        {
            await users.DeleteOneAsync(item => item.Username == username);
        }
    }

    [Theory]
    [InlineData("player", "new-player@example.com")]
    [InlineData("new-player", "player@unisearch.local")]
    public async Task Register_WithExistingUsernameOrEmail_ReturnsConflict(string username, string email)
    {
        using var client = _factory.CreateClient();
        var users = GetUsersCollection();
        var countBefore = await users.CountDocumentsAsync(FilterDefinition<User>.Empty);
        var request = new RegisterRequest
        {
            Username = username,
            Email = email,
            Password = "ValidPassword1!"
        };

        var response = await client.PostAsJsonAsync("api/auth/register", request);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        var countAfter = await users.CountDocumentsAsync(FilterDefinition<User>.Empty);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal(DomainApiErrors.UserAlreadyExists.ErrorCode, error.ErrorCode);
        Assert.Equal(DomainApiErrors.UserAlreadyExists.Message, error.Message);
        Assert.Equal(countBefore, countAfter);
    }

    [Theory]
    [InlineData("invalid-email", "ValidPassword1!")]
    [InlineData("player@example.com", "short")]
    public async Task Register_WithInvalidRequest_ReturnsBadRequest(string email, string password)
    {
        using var client = _factory.CreateClient();
        var users = GetUsersCollection();
        var username = $"player-{Guid.NewGuid():N}";
        var request = new RegisterRequest { Username = username, Email = email, Password = password };

        var response = await client.PostAsJsonAsync("api/auth/register", request);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        var user = await users.Find(item => item.Username == username).FirstOrDefaultAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal(GeneralApiErrors.BadRequest.ErrorCode, error.ErrorCode);
        Assert.Null(user);
    }

    [Theory]
    [InlineData("admin", "SEED_ADMIN_PASSWORD")]
    [InlineData("admin@unisearch.local", "SEED_ADMIN_PASSWORD")]
    [InlineData("player", "SEED_PLAYER_PASSWORD")]
    public async Task Login_WithValidCredentials_ReturnsUsableToken(string identifier, string passwordVariable)
    {
        using var client = _factory.CreateClient();
        var password = Environment.GetEnvironmentVariable(passwordVariable)
            ?? throw new InvalidOperationException($"{passwordVariable} n'est pas renseigné.");

        var response = await client.PostAsJsonAsync(
            "api/auth/login",
            new LoginRequest { Identifier = identifier, Password = password });
        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(login);
        Assert.False(string.IsNullOrWhiteSpace(login.Token));

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", login.Token);
        var protectedResponse = await client.GetAsync(
            passwordVariable == "SEED_PLAYER_PASSWORD" ? "api/packs/player/test" : "api/grids");
        Assert.Equal(HttpStatusCode.OK, protectedResponse.StatusCode);
    }

    [Theory]
    [InlineData("admin", "WrongPassword1!")]
    [InlineData("unknown-user", "ValidPassword1!")]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized(string identifier, string password)
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "api/auth/login",
            new LoginRequest { Identifier = identifier, Password = password });
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal(GeneralApiErrors.Unauthorized.ErrorCode, error.ErrorCode);
        Assert.Equal(GeneralApiErrors.Unauthorized.Message, error.Message);
    }

    [Fact]
    public async Task Login_WithMissingPassword_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "api/auth/login",
            new LoginRequest { Identifier = "admin", Password = string.Empty });
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(error);
        Assert.Equal(GeneralApiErrors.BadRequest.ErrorCode, error.ErrorCode);
    }

    private IMongoCollection<User> GetUsersCollection()
    {
        var database = _factory.Services.GetRequiredService<IMongoDatabase>();
        return database.GetCollection<User>("users");
    }
}
