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

    public async Task<AdminGridResponse?> GetGridByIdAsync(string gridId)
    {
        var grid = await _gridRepository.FindByIdAsync(gridId);

        return grid is null
            ? null
            : ToGridResponse(grid);
    }

    public async Task<List<AdminGridSummaryResponse>> GetAllGridsAsync()
    {
        var grids = await _gridRepository.FindAllAsync();

        return grids.Select(grid => new AdminGridSummaryResponse
        {
            Id = grid.Id,
            Size = grid.Size,
            Difficulty = grid.Difficulty,
            Pack = ToPackResponse(grid.Pack),
            WordCount = grid.Words.Count,
        }).ToList();
    }

    private static AdminGridResponse ToGridResponse(Grid grid)
    {
        return new AdminGridResponse
        {
            Id = grid.Id,
            Size = grid.Size,
            Letters = [.. grid.Letters],
            Difficulty = grid.Difficulty,
            Words = [.. grid.Words],
            Pack = ToPackResponse(grid.Pack),
        };
    }

    private static PackResponse? ToPackResponse(Grid.PackDetails? pack)
    {
        return pack is null
            ? null
            : new PackResponse
            {
                Name = pack.Name,
                Date = pack.Date,
            };
    }
}
