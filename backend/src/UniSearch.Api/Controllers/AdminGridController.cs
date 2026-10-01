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
public sealed class AdminGridController : ControllerBase
{
    private readonly GridCreationService _gridCreationService;
    private readonly GridService _gridService;

    public AdminGridController(GridCreationService gridCreationService, GridService gridService)
    {
        _gridCreationService = gridCreationService;
        _gridService = gridService;
    }

    [Route("api/grids")]
    [HttpPost]
    [Authorize(Roles = nameof(Role.Admin))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateGrid([FromBody] GridCreationRequest request)
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(GeneralApiErrors.Unauthorized);
        }

        if (request == null)
        {
            return BadRequest(GeneralApiErrors.BadRequest);
        }

        var result = await _gridCreationService.CreateGridAsync(request);

        return result switch
        {
            GridCreationResult.GridCreated => StatusCode(StatusCodes.Status201Created, new
                {
                    message = "Grille créée avec succès.",
                }),
            GridCreationResult.IncorrectGridSize => UnprocessableEntity(DomainApiErrors.IncorrectGridSize),
            GridCreationResult.MissingDifficulty => UnprocessableEntity(DomainApiErrors.MissingDifficulty),
            GridCreationResult.NotEnoughWords => UnprocessableEntity(DomainApiErrors.NotEnoughWords),
            GridCreationResult.InvalidWords => UnprocessableEntity(DomainApiErrors.InvalidWords),
            GridCreationResult.DuplicateWords => UnprocessableEntity(DomainApiErrors.DuplicateWords),
            GridCreationResult.PlacementFailed => UnprocessableEntity(DomainApiErrors.PlacementFailed),
            _ => throw new InvalidOperationException("Résultat inattendu lors de la création de la grille."),
        };
    }

    [Route("api/grids")]
    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin))]
    [ProducesResponseType(typeof(List<AdminGridSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllGridsAsync()
    {
        var grids = await _gridService.GetAllGridsAsync();
        return Ok(grids);
    }

    [Route("api/grids/{gridId}")]
    [HttpGet]
    [Authorize(Roles = nameof(Role.Admin))]
    [ProducesResponseType(typeof(AdminGridResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGridByIdAsync(string gridId)
    {
        if (!ObjectId.TryParse(gridId, out _))
        {
            return BadRequest(GeneralApiErrors.BadRequest);
        }

        var grid = await _gridService.GetGridByIdAsync(gridId);

        if (grid is null)
        {
            return NotFound(DomainApiErrors.GridNotFound);
        }

        return Ok(grid);
    }
}
