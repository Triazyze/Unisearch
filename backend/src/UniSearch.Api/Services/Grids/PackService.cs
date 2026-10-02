using UniSearch.Api.Models.Dtos.Grids;
using UniSearch.Api.Repositories;

namespace UniSearch.Api.Services.Grids;

public sealed class PackService
{
    private const string TestPackName = "test";

    private readonly GridRepository _gridRepository;

    public PackService(GridRepository gridRepository)
    {
        _gridRepository = gridRepository;
    }

    public async Task<PlayerPackResponse?> GetTestPackAsync()
    {
        var testGrids = await _gridRepository.FindByPackNameAsync(TestPackName);
        if (testGrids.Count != 2)
        {
            return null;
        }

        return new PlayerPackResponse
        {
            Pack = new PackResponse
            {
                Name = TestPackName,
                Date = null,
            },
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
