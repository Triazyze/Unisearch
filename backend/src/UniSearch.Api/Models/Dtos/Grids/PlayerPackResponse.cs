namespace UniSearch.Api.Models.Dtos.Grids;

public sealed class PlayerPackResponse
{
    required public PackResponse Pack { get; init; }

    public List<PlayerGridResponse> Grids { get; init; } = [];
}
