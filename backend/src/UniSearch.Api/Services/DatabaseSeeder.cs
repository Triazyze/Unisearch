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
    private const string TestPackId = "66f000000000000000000010";

    private static readonly Grid FirstTestGrid = new ()
    {
        Id = "66f000000000000000000001",
        Size = 4,
        Letters =
        [
            "CHAT",
            "XXXX",
            "XXXX",
            "XXXX",
        ],
        Words =
        [
            "CHAT",
        ],
        Difficulty = "Test",
    };

    private static readonly Grid SecondTestGrid = new ()
    {
        Id = "66f000000000000000000002",
        Size = 6,
        Letters =
        [
            "CHATXX",
            "LXXXXC",
            "XIXXXH",
            "XXOXXI",
            "XXXNXE",
            "XXXXXN",
        ],
        Words =
        [
            "CHAT",
            "CHIEN",
            "LION",
        ],
        Difficulty = "Test",
    };

    private readonly UserRepository _userRepository;
    private readonly GridRepository _gridRepository;
    private readonly PackRepository _packRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public DatabaseSeeder(
        UserRepository userRepository,
        GridRepository gridRepository,
        PackRepository packRepository,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _gridRepository = gridRepository;
        _packRepository = packRepository;
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

        await SeedGridAsync(FirstTestGrid);
        await SeedGridAsync(SecondTestGrid);
        await SeedTestPackAsync();
    }

    private static bool IsExpectedGrid(Grid existingGrid, Grid expectedGrid)
    {
        return existingGrid.Id == expectedGrid.Id
            && existingGrid.Size == expectedGrid.Size
            && existingGrid.Letters.SequenceEqual(expectedGrid.Letters)
            && existingGrid.Words.SequenceEqual(expectedGrid.Words)
            && existingGrid.Difficulty == expectedGrid.Difficulty;
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

    private async Task SeedGridAsync(Grid expectedGrid)
    {
        var existingGrid = await _gridRepository.FindByIdAsync(expectedGrid.Id);
        if (existingGrid is null)
        {
            await _gridRepository.CreateAsync(expectedGrid);
            return;
        }

        if (IsExpectedGrid(existingGrid, expectedGrid))
        {
            return;
        }

        throw new InvalidOperationException(
            "Le seed ne peut pas créer la grille " + expectedGrid.Id + ", car son identifiant est déjà utilisé.");
    }

    private async Task SeedTestPackAsync()
    {
        var existingPack = await _packRepository.FindByIdAsync(TestPackId);
        if (existingPack is null)
        {
            var testPack = new Pack
            {
                Id = TestPackId,
                GridIds = [FirstTestGrid.Id, SecondTestGrid.Id],
            };

            await _packRepository.CreateAsync(testPack);
            return;
        }

        if (existingPack.GridIds.SequenceEqual([FirstTestGrid.Id, SecondTestGrid.Id]))
        {
            return;
        }

        throw new InvalidOperationException(
            "Le seed ne peut pas créer le pack de test, car son identifiant est déjà utilisé.");
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
