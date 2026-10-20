using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UniSearch.Api.Models.Dtos.Authentication;

namespace UniSearch.IntegrationTests.Helpers;

public static class AuthenticationHelper
{
    public static async Task AuthenticatePlayerAsync(HttpClient client)
    {
        await AuthenticateAsync(client, "player", "SEED_PLAYER_PASSWORD");
    }

    public static async Task AuthenticateAdminAsync(HttpClient client)
    {
        await AuthenticateAsync(client, "admin", "SEED_ADMIN_PASSWORD");
    }

    private static async Task AuthenticateAsync(
        HttpClient client,
        string identifier,
        string passwordVariable)
    {
        var password = Environment.GetEnvironmentVariable(passwordVariable)
            ?? throw new InvalidOperationException($"{passwordVariable} n'est pas renseigné.");

        var response = await client.PostAsJsonAsync(
            "api/auth/login",
            new LoginRequest
            {
                Identifier = identifier,
                Password = password
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>()
            ?? throw new InvalidOperationException("Impossible de récupérer le token d'authentification.");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.Token);
    }
}
