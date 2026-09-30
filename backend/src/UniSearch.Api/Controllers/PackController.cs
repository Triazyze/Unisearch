using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSearch.Api.Errors;
using UniSearch.Api.Models.Entities;
using UniSearch.Api.Models.Dtos;
using UniSearch.Api.Services.Grids;

namespace UniSearch.Api.Controllers;

[ApiController]
[Route("api/packs/player/test")]
public sealed class PackController : ControllerBase
{
    private readonly PackService _packService;

    public PackController(PackService packService)
    {
        _packService = packService;
    }

    [HttpGet]
    [Authorize(Roles = nameof(Role.Player))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTestPackAsync()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiErrors.Unauthorized);
        }

        var pack = await _packService.GetTestPackAsync();

        if (pack is null)
        {
            return NotFound(ApiErrors.TestPackNotFound);
        }

        return Ok(pack);
    }
}
