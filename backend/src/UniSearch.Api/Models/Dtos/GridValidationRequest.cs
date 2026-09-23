namespace UniSearch.Api.Models.Dtos;

public sealed class GridValidationRequest
{
    required public List<string> Letters { get; set; }

    required public List<string> Words { get; set; }

    required public int Time { get; set; }

    required public int TraceAttempts { get; set; }

    required public List<Trace> Traces { get; set; }
}
