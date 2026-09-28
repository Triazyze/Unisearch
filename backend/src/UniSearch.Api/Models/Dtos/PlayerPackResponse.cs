namespace UniSearch.Api.Models.Dtos;

public sealed class PlayerPackResponse
{
    public DateTime? PackDate { get; init; }

    public List<PlayerGridResponse> Grids { get; init; } = [];
}
