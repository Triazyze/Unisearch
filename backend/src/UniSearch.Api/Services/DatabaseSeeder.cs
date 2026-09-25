using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using UniSearch.Api.Models.Entities;
using UniSearch.Api.Repositories;

namespace UniSearch.Api.Services;

public sealed class DatabaseSeeder
{
    private const string PlayerUsername = "player";
    private const string PlayerEmail = "player@unisearch.local";
    private const string AdminUsername = "admin";
    private const string AdminEmail = "admin@unisearch.local";
    private const string VerificationGridId = "66f000000000000000000001";
    private const int VerificationGridSize = 4;
    private const string VerificationGridDifficulty = "Test";

    private static readonly List<string> VerificationGridLetters =
    [
        "CHAT",
        "XXXX",
        "XXXX",
        "XXXX",
    ];

    private static readonly List<string> VerificationGridWords =
    [
        "CHAT",
    ];

    private readonly UserRepository _userRepository;
    private readonly GridRepository _gridRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public DatabaseSeeder(
        UserRepository userRepository,
        GridRepository gridRepository,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _gridRepository = gridRepository;
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

        await SeedVerificationGridAsync();
    }

    private static bool IsExpectedVerificationGrid(Grid grid)
    {
        return grid.Id == VerificationGridId
            && grid.Size == VerificationGridSize
            && grid.Letters.SequenceEqual(VerificationGridLetters)
            && grid.Words.SequenceEqual(VerificationGridWords)
            && grid.Difficulty == VerificationGridDifficulty;
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

    private async Task SeedVerificationGridAsync()
    {
        var existingGrid = await _gridRepository.FindByIdAsync(VerificationGridId);
        if (existingGrid is null)
        {
            var grid = new Grid
            {
                Id = VerificationGridId,
                Size = VerificationGridSize,
                Letters = new List<string>(VerificationGridLetters),
                Words = new List<string>(VerificationGridWords),
                Difficulty = VerificationGridDifficulty,
            };

            await _gridRepository.CreateAsync(grid);
            return;
        }

        if (IsExpectedVerificationGrid(existingGrid))
        {
            return;
        }

        throw new InvalidOperationException(
            "Le seed ne peut pas créer la grille de vérification, car son identifiant est déjà utilisé.");
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
