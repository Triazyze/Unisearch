using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSearch.Api.Models.Entities;
using UniSearch.Api.Models.Dtos;
using UniSearch.Api.Services;

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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTestPackAsync()
    {
        var pack = await _packService.GetTestPackAsync();

        if (pack is null)
        {
            return NotFound(new
            {
                message = "Le pack de test est introuvable.",
            });
        }

        return Ok(pack);
    }
}
