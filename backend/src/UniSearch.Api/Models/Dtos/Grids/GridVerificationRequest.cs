namespace UniSearch.Api.Models.Dtos.Grids;

public sealed class GridVerificationRequest
{
    required public List<string> Letters { get; set; }

    required public List<string> Words { get; set; }

    required public int Time { get; set; }

    required public int TraceAttempts { get; set; }

    required public List<Trace> Traces { get; set; }
}
