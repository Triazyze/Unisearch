using UniSearch.Api.Models.Dtos;
using UniSearch.Api.Repositories;

namespace UniSearch.Api.Services;

public sealed class PackService
{
    private readonly PackRepository _packRepository;
    private readonly GridRepository _gridRepository;

    public PackService(PackRepository packRepository, GridRepository gridRepository)
    {
        _packRepository = packRepository;
        _gridRepository = gridRepository;
    }

    public async Task<PlayerPackResponse?> GetTestPackAsync()
    {
        var pack = await _packRepository.FindByIdAsync("66f000000000000000000010");
        if (pack is null)
        {
            return null;
        }

        var grids = new List<PlayerGridResponse>();

        foreach (var gridId in pack.GridIds)
        {
            var grid = await _gridRepository.FindByIdAsync(gridId);
            if (grid is null)
            {
                return null;
            }

            grids.Add(new PlayerGridResponse
            {
                Id = grid.Id,
                Size = grid.Size,
                Letters = [.. grid.Letters],
                Words = [.. grid.Words],
                Difficulty = grid.Difficulty,
            });
        }

        return new PlayerPackResponse
        {
            Id = pack.Id,
            Grids = grids,
        };
    }
}
