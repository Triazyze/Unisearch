namespace UniSearch.Api.Models.Dtos.Grids;

public sealed class GridResponse
{
    public string Id { get; init; } = string.Empty;

    public int Size { get; init; }

    public List<string> Letters { get; init; } = [];

    public List<string> Words { get; init; } = [];

    public string Difficulty { get; init; } = string.Empty;
}
