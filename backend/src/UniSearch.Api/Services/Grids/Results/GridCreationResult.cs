namespace UniSearch.Api.Services.Grids.Results;

public enum GridCreationResult
{
    GridCreated,
    MissingDifficulty,
    IncorrectGridSize,
    InvalidWords,
    DuplicateWords,
    NotEnoughWords,
    PlacementFailed,
}
