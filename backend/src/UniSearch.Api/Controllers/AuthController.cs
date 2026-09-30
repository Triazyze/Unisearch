using Microsoft.AspNetCore.Mvc;
using UniSearch.Api.Errors;
using UniSearch.Api.Models.Dtos;
using UniSearch.Api.Models.Dtos.Authentication;
using UniSearch.Api.Services.Authentication;
using UniSearch.Api.Services.Authentication.Results;

namespace UniSearch.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(
            request.Username,
            request.Email,
            request.Password);

        return result switch
        {
            RegisterUserResult.Created => StatusCode(StatusCodes.Status201Created, new
            {
                message = $"Joueur {request.Username} a été créé avec succès.",
            }),
            RegisterUserResult.Duplicate => Conflict(new
            {
                message = "Utilisateur ou email déjà existant.",
            }),
            _ => throw new InvalidOperationException("Resultat inattendu lors de l'enregistrement de l'utilisateur."),
        };
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await _authService.LoginAsync(
            request.Identifier,
            request.Password);

        if (token is null)
        {
            return Unauthorized(ApiErrors.Unauthorized);
        }

        return Ok(new LoginResponse
        {
            Token = token,
        });
    }
}
