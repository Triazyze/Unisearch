namespace UniSearch.Api.Models.Dtos.Grids;

public sealed class AdminGridSummaryResponse
{
    public string Id { get; init; } = string.Empty;

    public int Size { get; init; }

    public string Difficulty { get; init; } = string.Empty;

    public DateTime UpdatedAt { get; init; }

    public PackResponse? Pack { get; init; }

    public int WordCount { get; init; }
}
