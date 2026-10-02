namespace UniSearch.Api.Models.Dtos.Grids;

public sealed class Trace
{
    required public string Word { get; set; }

    required public int StartRow { get; set; }

    required public int StartColumn { get; set; }

    required public int EndRow { get; set; }

    required public int EndColumn { get; set; }
}
