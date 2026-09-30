namespace UniSearch.Api.Models.Dtos.Grids;

public sealed class GridCreationRequest
{
    required public int Size { get; set; }

    required public List<string> Words { get; set; }

    required public string Difficulty { get; set; }
}
