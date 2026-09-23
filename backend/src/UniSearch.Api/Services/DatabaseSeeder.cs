using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using UniSearch.Api.Models;
using UniSearch.Api.Repositories;

namespace UniSearch.Api.Services;

public sealed class DatabaseSeeder
{
    private const string PlayerUsername = "player";
    private const string PlayerEmail = "player@unisearch.local";
    private const string AdminUsername = "admin";
    private const string AdminEmail = "admin@unisearch.local";

    private readonly UserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public DatabaseSeeder(
        UserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        if (!_configuration.GetValue<bool>("SEED_ENABLED"))
        {
            return;
        }

        await SeedUserAsync(
            PlayerUsername,
            PlayerEmail,
            GetRequiredConfigurationValue("SEED_PLAYER_PASSWORD"),
            Role.Player);

        await SeedUserAsync(
            AdminUsername,
            AdminEmail,
            GetRequiredConfigurationValue("SEED_ADMIN_PASSWORD"),
            Role.Admin);
    }

    private static bool IsExpectedSeedUser(User? userByUsername, User? userByEmail, string username, string email, Role role)
    {
        return userByUsername is not null
            && userByEmail is not null
            && userByUsername.Id == userByEmail.Id
            && userByUsername.Username == username
            && userByUsername.Email == email
            && userByUsername.Role == role;
    }

    private async Task SeedUserAsync(
        string username,
        string email,
        string password,
        Role role)
    {
        var userByUsername = await _userRepository
            .FindByUsernameOrEmailAsync(username);
        var userByEmail = await _userRepository
            .FindByUsernameOrEmailAsync(email);

        if (userByUsername is null && userByEmail is null)
        {
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = string.Empty,
                Role = role,
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            await _userRepository.CreateAsync(user);
            return;
        }

        if (IsExpectedSeedUser(userByUsername, userByEmail, username, email, role))
        {
            return;
        }

        throw new InvalidOperationException(
            $"Le seed ne peut pas créer le compte {username}, car ses identifiants sont déjà utilisés.");
    }

    private string GetRequiredConfigurationValue(string key)
    {
        var value = _configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"{key} doit être défini lorsque SEED_ENABLED est activé.");
        }

        return value;
    }
}
