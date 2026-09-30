using UniSearch.Api.Models.Dtos.Grids;
using UniSearch.Api.Models.Entities;
using UniSearch.Api.Repositories;
using UniSearch.Api.Services.Grids.Results;

namespace UniSearch.Api.Services.Grids;

public sealed class GridVerificationService
{
    private readonly GridRepository _gridRepository;
    private readonly PlayerResultRepository _playerResultRepository;

    public GridVerificationService(GridRepository gridRepository, PlayerResultRepository playerResultRepository)
    {
        _gridRepository = gridRepository;
        _playerResultRepository = playerResultRepository;
    }

    public async Task<GridVerificationResult> VerifyGridAsync(string gridId, string userId, GridVerificationRequest request)
    {
        var grid = await _gridRepository.FindByIdAsync(gridId);
        if (grid is null)
        {
            return GridVerificationResult.GridNotFound;
        }

        var submittedLetters = request.Letters;
        var submittedWords = request.Words;
        var submittedTraces = request.Traces;

        // Vérification des lettres
        if (!AreLettersValid(grid, submittedLetters))
        {
            return GridVerificationResult.LettersInvalid;
        }

        // Vérification des mots
        if (!AreWordsValid(grid, submittedWords))
        {
            return GridVerificationResult.WordsInvalid;
        }

        if (!AreTracesMatchingWords(grid, submittedTraces))
        {
            return GridVerificationResult.TraceMismatch;
        }

        if (!AreTracesValid(grid, submittedTraces))
        {
            return GridVerificationResult.TracesInvalid;
        }

        // Si toutes les vérifications sont réussies, on enregistre le résultat du joueur
        var newPlayerResult = new PlayerResult
        {
            UserId = userId,
            GridId = gridId,
            Time = request.Time,
            TraceAttempts = request.TraceAttempts,
        };
        var existingPlayerResult = await _playerResultRepository.FindByUserIdAndGridIdAsync(userId, gridId);

        if (existingPlayerResult is null)
        {
            await _playerResultRepository.CreateAsync(newPlayerResult);
        }
        else
        {
            newPlayerResult.Id = existingPlayerResult.Id;
            await _playerResultRepository.ReplaceAsync(newPlayerResult);
        }

        return GridVerificationResult.Validated;
    }

    private static bool AreLettersValid(Grid grid, List<string> submittedLetters)
    {
        return grid.Letters.SequenceEqual(submittedLetters);
    }

    private static bool AreWordsValid(Grid grid, List<string> submittedWords)
    {
        return grid.Words.OrderBy(word => word).SequenceEqual(submittedWords.OrderBy(word => word));
    }

    private static bool AreTracesMatchingWords(Grid grid, List<Trace> submittedTraces)
    {
        return grid.Words.Count == submittedTraces.Count && grid.Words.All(word => submittedTraces.Any(trace => trace.Word == word));
    }

    private static bool IsCoordinateInsideGrid(int row, int column, int gridSize)
    {
        return row >= 0
            && row < gridSize
            && column >= 0
            && column < gridSize;
    }

    private static bool AreTracesValid(Grid grid, List<Trace> submittedTraces)
    {
        foreach (var trace in submittedTraces)
        {
            // Vérifie que les coordonnées de début et de fin sont à l'intérieur de la grille
            var hasValidCoordinates =
                IsCoordinateInsideGrid(trace.StartRow, trace.StartColumn, grid.Size)
                && IsCoordinateInsideGrid(trace.EndRow, trace.EndColumn, grid.Size);
            if (!hasValidCoordinates)
            {
                return false;
            }

            // Vérifie la direction du tracé
            var rowDelta = trace.EndRow - trace.StartRow;
            var columnDelta = trace.EndColumn - trace.StartColumn;
            if (rowDelta != 0 && columnDelta != 0 && Math.Abs(rowDelta) != Math.Abs(columnDelta))
            {
                return false;
            }

            // Vérifie que la longueur du tracé correspond à la longueur du mot
            if (Math.Max(Math.Abs(rowDelta), Math.Abs(columnDelta)) + 1 != trace.Word.Length)
            {
                return false;
            }

            // Vérifie que les lettres tracées correspondent au mot ou à son inverse
            var rowStep = Math.Sign(rowDelta);
            var columnStep = Math.Sign(columnDelta);
            var tracedLetters = new char[trace.Word.Length];

            for (var i = 0; i < trace.Word.Length; i++)
            {
                var row = trace.StartRow + (i * rowStep);
                var column = trace.StartColumn + (i * columnStep);
                tracedLetters[i] = grid.Letters[row][column];
            }

            var tracedWord = new string(tracedLetters);
            var reversedWord = new string(trace.Word.Reverse().ToArray());
            if (tracedWord != trace.Word && tracedWord != reversedWord)
            {
                return false;
            }
        }

        return true;
    }
}
