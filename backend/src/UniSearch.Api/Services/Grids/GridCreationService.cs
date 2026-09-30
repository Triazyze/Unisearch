using MongoDB.Driver;
using UniSearch.Api.Models.Dtos.Grids;
using UniSearch.Api.Models.Entities;
using UniSearch.Api.Repositories;
using UniSearch.Api.Services.Grids;
using UniSearch.Api.Services.Grids.Results;
using BSONObjectId = MongoDB.Bson.ObjectId;

namespace UniSearch.Api.Services.Grids;

public sealed class GridCreationService
{
    private readonly GridRepository _gridRepository;

    public GridCreationService(GridRepository gridRepository)
    {
        _gridRepository = gridRepository;
    }

    public async Task<GridCreationResult> CreateGridAsync(GridCreationRequest request)
    {
        // Valide la taille de la grille.
        if (request.Size < 4 || request.Size > 10)
        {
            return GridCreationResult.IncorrectGridSize;
        }

        // Valide la difficulté.
        if (string.IsNullOrWhiteSpace(request.Difficulty))
        {
            return GridCreationResult.MissingDifficulty;
        }

        // Valide la liste de mots.
        if (request.Words == null || request.Words.Count == 0)
        {
            return GridCreationResult.NotEnoughWords;
        }

        // Valide les mots
        foreach (var word in request.Words)
        {
            if (word.Length == 0 || word.Any(char.IsSurrogate))
            {
                return GridCreationResult.InvalidWords;
            }
        }

        // Vérifie que chaque mot est unique.
        foreach (var word in request.Words)
        {
            if (request.Words.Count(w => StringComparer.Ordinal.Equals(w, word)) > 1)
            {
                return GridCreationResult.DuplicateWords;
            }
        }

        // Vérifie que chaque mot peut entrer dans la grille.
        foreach (var word in request.Words)
        {
            if (word.Length > request.Size)
            {
                return GridCreationResult.PlacementFailed;
            }
        }

        // Vérifie que la somme des longueurs est admissible.
        var totalLength = request.Words.Sum(w => w.Length);
        if (totalLength > request.Size * request.Size)
        {
            return GridCreationResult.PlacementFailed;
        }

        // Initialise les données nécessaires au placement.
        var wordsToPlace = request.Words.OrderByDescending(word => word.Length).ToList();
        var gridLetters = new char[request.Size, request.Size];

        // Place tous les mots dans la grille.
        foreach (var word in wordsToPlace)
        {
            if (!PlaceWordInGrid(gridLetters, word))
            {
                return GridCreationResult.PlacementFailed;
            }
        }

        // Complète les cases restantes avec des lettres.
        var random = new Random();
        for (int i = 0; i < request.Size; i++)
        {
            for (int j = 0; j < request.Size; j++)
            {
                if (gridLetters[i, j] == '\0') // Si la case est vide
                {
                    gridLetters[i, j] = (char)('A' + random.Next(0, 26)); // Remplit avec une lettre aléatoire
                }
            }
        }

        // Vérifie que chaque mot cible apparaît une seule fois.
        foreach (var word in request.Words)
        {
            if (CountWordOccurrences(gridLetters, word) != 1)
            {
                return GridCreationResult.PlacementFailed;
            }
        }

        // Construit l'entité Grid avec Pack = null.
        var grid = new Grid
        {
            Id = BSONObjectId.GenerateNewId().ToString(),
            Size = request.Size,
            Letters = ConvertCellsToRows(gridLetters),
            Words = [.. request.Words],
            Difficulty = request.Difficulty,
            Pack = null,
        };
        // Persiste la grille uniquement après une génération réussie.
        await _gridRepository.CreateAsync(grid);
        // Retourne la grille créée.
        return GridCreationResult.GridCreated;
    }

    private static bool PlaceWordInGrid(char[,] grid, string word)
    {
        // Implémentez la logique pour placer le mot dans la grille.
        // Vous pouvez choisir une position aléatoire et une direction (horizontal, vertical, diagonal).
        // Assurez-vous que le mot ne chevauche pas d'autres mots déjà placés.
        // Retournez true si le placement est réussi, sinon false.
        return true; // Placeholder, remplacez par la logique réelle.
    }

    private static bool DoesWordMatchAt(
    char[,] grid,
    string word,
    int startRow,
    int startColumn,
    int rowDirection,
    int columnDirection)
    {
        for (var index = 0; index < word.Length; index++)
        {
            var row = startRow + (index * rowDirection);
            var column = startColumn + (index * columnDirection);

            if (row < 0
                || row >= grid.GetLength(0)
                || column < 0
                || column >= grid.GetLength(1)
                || grid[row, column] != word[index])
            {
                return false;
            }
        }

        return true;
    }

    private static int CountWordOccurrences(char[,] grid, string word)
    {
        var count = 0;
        if (word.Length == 1)
        {
            foreach (var letter in grid)
            {
                if (letter == word[0])
                {
                    count++;
                }
            }

            return count;
        }

        for (var row = 0; row < grid.GetLength(0); row++)
        {
            for (var column = 0; column < grid.GetLength(1); column++)
            {
                for (int i = 0; i < 8; i++)
                {
                    var direction = i switch
                    {
                        0 => (Row: -1, Column: 0),
                        1 => (Row: -1, Column: 1),
                        2 => (Row: 0, Column: 1),
                        3 => (Row: 1, Column: 1),
                        4 => (Row: 1, Column: 0),
                        5 => (Row: 1, Column: -1),
                        6 => (Row: 0, Column: -1),
                        _ => (Row: -1, Column: -1),
                    };

                    if (DoesWordMatchAt(
                        grid,
                        word,
                        row,
                        column,
                        direction.Row,
                        direction.Column))
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    private static List<string> ConvertCellsToRows(char[,] grid)
    {
        var rows = new List<string>();
        for (var i = 0; i < grid.GetLength(0); i++)
        {
            var row = new char[grid.GetLength(1)];
            for (var j = 0; j < grid.GetLength(1); j++)
            {
                row[j] = grid[i, j];
            }

            rows.Add(new string(row));
        }

        return rows;
    }
}
