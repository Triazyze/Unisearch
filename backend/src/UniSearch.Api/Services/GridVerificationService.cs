using UniSearch.Api.Models.Entities;
using UniSearch.Api.Models.Dtos;
using UniSearch.Api.Repositories;

namespace UniSearch.Api.Services;

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

        var letters = request.Letters;
        var words = request.Words;
        var traces = request.Traces;

        // Vérification des lettres
        if (!AreLettersValid(grid, letters))
        {
            return GridVerificationResult.LettersInvalid;
        }

        // Vérification des mots
        if (!AreWordsValid(grid, words))
        {
            return GridVerificationResult.WordsInvalid;
        }

        if (!AreTracesComplete(grid, traces))
        {
            return GridVerificationResult.TracesIncomplete;
        }

        if (!AreTracesValid(grid, traces))
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

    private static bool AreLettersValid(Grid grid, List<string> letters)
    {
        throw new NotImplementedException("La vérification des lettres n'est pas encore implémentée.");
    }

    private static bool AreWordsValid(Grid grid, List<string> words)
    {
        throw new NotImplementedException("La vérification des mots n'est pas encore implémentée.");
    }

    private static bool AreTracesComplete(Grid grid, List<Trace> traces)
    {
        throw new NotImplementedException("La vérification de la complétude des traces n'est pas encore implémentée.");
    }

    private static bool AreTracesValid(Grid grid, List<Trace> traces)
    {
        throw new NotImplementedException("La vérification de la validité des traces n'est pas encore implémentée.");
    }
}
