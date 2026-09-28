using UniSearch.Api.Models.Dtos;
using UniSearch.Api.Repositories;

namespace UniSearch.Api.Services;

public sealed class PackService
{
    private readonly GridRepository _gridRepository;

    public PackService(GridRepository gridRepository)
    {
        _gridRepository = gridRepository;
    }

    public async Task<PlayerPackResponse?> GetTestPackAsync()
    {
        var testGrids = await _gridRepository.FindByPackDateAsync(null);
        if (testGrids.Count != 2)
        {
            return null;
        }

        return new PlayerPackResponse
        {
            PackDate = null,
            Grids = testGrids.Select(grid => new PlayerGridResponse
            {
                Id = grid.Id,
                Size = grid.Size,
                Letters = [.. grid.Letters],
                Words = [.. grid.Words],
                Difficulty = grid.Difficulty,
            }).ToList(),
        };
    }
}
