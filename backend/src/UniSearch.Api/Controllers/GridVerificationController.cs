using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSearch.Api.Errors;
using UniSearch.Api.Models.Dtos;
using UniSearch.Api.Models.Entities;
using UniSearch.Api.Services;
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> VerifyGrid(string gridId, [FromBody] GridVerificationRequest request)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiErrors.Unauthorized);
        }

        if (!ObjectId.TryParse(gridId, out _))
        {
            return BadRequest(ApiErrors.BadRequest);
        }

        var result = await _gridVerificationService.VerifyGridAsync(gridId, userId, request);

        return result switch
        {
            GridVerificationResult.Validated => Ok(new
            {
                message = "Grille vérifiée avec succès.",
            }),
            GridVerificationResult.GridNotFound => NotFound(new
            {
                message = "Grille non trouvée.",
            }),
            GridVerificationResult.LettersInvalid => UnprocessableEntity(new
            {
                message = "Les lettres soumises ne correspondent pas à la grille.",
            }),
            GridVerificationResult.WordsInvalid => UnprocessableEntity(new
            {
                message = "Les mots soumis ne correspondent pas à la grille.",
            }),
            GridVerificationResult.TraceMismatch => UnprocessableEntity(new
            {
                message = "Les tracés soumis ne correspondent pas aux mots.",
            }),
            GridVerificationResult.TracesInvalid => UnprocessableEntity(new
            {
                message = "Les tracés soumis sont invalides.",
            }),
            _ => throw new InvalidOperationException("Résultat inattendu lors de la vérification de la grille."),
        };
    }
}
