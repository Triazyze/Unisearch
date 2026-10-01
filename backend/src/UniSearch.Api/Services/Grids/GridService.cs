using UniSearch.Api.Models.Dtos.Grids;
using UniSearch.Api.Models.Entities;
using UniSearch.Api.Repositories;

namespace UniSearch.Api.Services.Grids;

public sealed class GridService
{
    private readonly GridRepository _gridRepository;

    public GridService(GridRepository gridRepository)
    {
        _gridRepository = gridRepository;
    }

    public async Task<GridResponse?> GetGridByIdAsync(string gridId)
    {
        var grid = await _gridRepository.FindByIdAsync(gridId);

        return grid is null
            ? null
            : ToGridResponse(grid);
    }

    public async Task<List<GridResponse>> GetAllGridsAsync()
    {
        var grids = await _gridRepository.FindAllAsync();

        return grids.Select(ToGridResponse).ToList();
    }

    private static GridResponse ToGridResponse(Grid grid)
    {
        return new GridResponse
        {
            Id = grid.Id,
            Size = grid.Size,
            Letters = grid.Letters,
            Difficulty = grid.Difficulty,
            Words = grid.Words,
        };
    }
}
