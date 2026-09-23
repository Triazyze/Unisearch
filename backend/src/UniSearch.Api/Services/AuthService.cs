using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using UniSearch.Api.Models.Entities;
using UniSearch.Api.Repositories;

namespace UniSearch.Api.Services;

public sealed class AuthService
{
    private readonly UserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JwtService _jwtService;

    public AuthService(
        UserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        JwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<RegisterUserResult> RegisterAsync(string username, string email, string password)
    {
        var normalizedUsername = username.Trim();
        var normalizedEmail = email.Trim();

        if (await _userRepository.ExistsByUsernameOrEmailAsync(
            normalizedUsername,
            normalizedEmail))
        {
            return RegisterUserResult.Duplicate;
        }

        var user = new User
        {
            Username = normalizedUsername,
            Email = normalizedEmail,
            PasswordHash = string.Empty,
            Role = Role.Player,
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        try
        {
            await _userRepository.CreateAsync(user);
            return RegisterUserResult.Created;
        }
        catch (MongoWriteException exception)
            when (exception.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            return RegisterUserResult.Duplicate;
        }
    }

    public async Task<string?> LoginAsync(string identifier, string password)
    {
        var normalizedIdentifier = identifier.Trim();
        var user = await _userRepository.FindByUsernameOrEmailAsync(normalizedIdentifier);
        if (user is null)
        {
            return null;
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return _jwtService.GenerateToken(user);
    }
}
