using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSearch.Api.Errors;
using UniSearch.Api.Models.Dtos;
using UniSearch.Api.Models.Dtos.Grids;
using UniSearch.Api.Models.Entities;
using UniSearch.Api.Services.Grids;
using UniSearch.Api.Services.Grids.Results;
using MongoDB.Bson;

namespace UniSearch.Api.Controllers;

[ApiController]
[Route("api/grid/{gridId}/verification")]
public sealed class GridVerificationController : ControllerBase
{
    private readonly GridVerificationService _gridVerificationService;

    public GridVerificationController(GridVerificationService gridVerificationService)
    {
        _gridVerificationService = gridVerificationService;
    }

    [HttpPost]
    [Authorize(Roles = nameof(Role.Player))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> VerifyGrid(string gridId, [FromBody] GridVerificationRequest request)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(GeneralApiErrors.Unauthorized);
        }

        if (!ObjectId.TryParse(gridId, out _))
        {
            return BadRequest(GeneralApiErrors.BadRequest);
        }

        var result = await _gridVerificationService.VerifyGridAsync(gridId, userId, request);

        return result switch
        {
            GridVerificationResult.Validated => Ok(new
            {
                message = "Grille vérifiée avec succès.",
            }),
            GridVerificationResult.GridNotFound => NotFound(DomainApiErrors.GridNotFound),
            GridVerificationResult.LettersInvalid => UnprocessableEntity(DomainApiErrors.LettersInvalid),
            GridVerificationResult.WordsInvalid => UnprocessableEntity(DomainApiErrors.WordsInvalid),
            GridVerificationResult.TraceMismatch => UnprocessableEntity(DomainApiErrors.TraceMismatch),
            GridVerificationResult.TracesInvalid => UnprocessableEntity(DomainApiErrors.TracesInvalid),
            _ => throw new InvalidOperationException("Résultat inattendu lors de la vérification de la grille."),
        };
    }
}
