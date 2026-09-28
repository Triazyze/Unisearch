namespace UniSearch.Api.Models.Dtos;

public sealed class PlayerPackResponse
{
    public string Id { get; init; } = string.Empty;

    public List<PlayerGridResponse> Grids { get; init; } = [];
}
