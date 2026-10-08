using UniSearch.Api.Models.Dtos.Grids;
using UniSearch.Api.Models.Entities;
using UniSearch.Api.Repositories;
using UniSearch.Api.Services.Grids.Results;
using BSONObjectId = MongoDB.Bson.ObjectId;

namespace UniSearch.Api.Services.Grids;

public sealed class GridCreationService
{
    private static readonly (int Row, int Column)[] Directions =
    [
        (-1, 0),  // Haut
        (-1, 1),  // Haut-droite
        (0, 1),   // Droite
        (1, 1),   // Bas-droite
        (1, 0),   // Bas
        (1, -1),  // Bas-gauche
        (0, -1),  // Gauche
        (-1, -1), // Haut-gauche
    ];

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
            if (string.IsNullOrEmpty(word) || word.Length <= 2 || word.Any(char.IsSurrogate))
            {
                return GridCreationResult.InvalidWords;
            }

            var reversedWord = new string(word.Reverse().ToArray());

            if (StringComparer.Ordinal.Equals(word, reversedWord))
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
        var maxAttempts = request.Size * request.Size;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Chaque tentative repart d'une grille vide.
            var lettersInGrid = new char[request.Size, request.Size];
            var placementSucceeded = true;

            // Place tous les mots dans la grille.
            foreach (var word in wordsToPlace)
            {
                if (!PlaceWordInGrid(lettersInGrid, word))
                {
                    placementSucceeded = false;
                    break;
                }
            }

            // Un mot n'a pas pu être placé :
            // abandonne cette grille et passe à la tentative suivante.
            if (!placementSucceeded)
            {
                continue;
            }

            // Complète les cases restantes avec des lettres aléatoires.
            for (var row = 0; row < request.Size; row++)
            {
                for (var column = 0; column < request.Size; column++)
                {
                    if (lettersInGrid[row, column] == '\0')
                    {
                        lettersInGrid[row, column] =
                            (char)('A' + Random.Shared.Next(0, 26));
                    }
                }
            }

            // Vérifie que chaque mot cible apparaît exactement une fois.
            var occurrencesAreValid = true;

            foreach (var word in request.Words)
            {
                if (CountWordOccurrences(lettersInGrid, word) != 1)
                {
                    occurrencesAreValid = false;
                    break;
                }
            }

            // Le remplissage a créé une occurrence supplémentaire :
            // abandonne cette grille et recommence.
            if (!occurrencesAreValid)
            {
                continue;
            }

            // La génération est valide : construit et persiste la grille.
            var finalGrid = new Grid
            {
                Id = BSONObjectId.GenerateNewId().ToString(),
                Size = request.Size,
                Letters = ConvertCellsToRows(lettersInGrid),
                Words = [.. request.Words],
                Difficulty = request.Difficulty,
                UpdatedAt = DateTime.UtcNow,
                Pack = null,
            };

            await _gridRepository.CreateAsync(finalGrid);

            return GridCreationResult.GridCreated;
        }

        // Toutes les tentatives ont échoué.
        return GridCreationResult.PlacementFailed;
    }

    // Vérifie si une position donnée est à l'intérieur des limites de la grille.
    private static bool IsWithinBounds(char[,] grid, int row, int column)
    {
        if (row < 0 || row >= grid.GetLength(0) || column < 0 || column >= grid.GetLength(1))
        {
            return false;
        }

        return true;
    }

    // Vérifie si un mot correspond à une position spécifique dans la grille.
    private static bool DoesWordMatchAt(char[,] grid, string word, int startRow, int startColumn, int rowDirection, int columnDirection)
    {
        for (var index = 0; index < word.Length; index++)
        {
            var row = startRow + (index * rowDirection);
            var column = startColumn + (index * columnDirection);

            if (!IsWithinBounds(grid, row, column) || grid[row, column] != word[index])
            {
                return false;
            }
        }

        return true;
    }

    // Vérifie si un mot peut être placé à une position spécifique dans la grille.
    private static bool CanPlaceWordAt(char[,] grid, string word, int startRow, int startColumn, int rowDirection, int columnDirection)
    {
        for (var index = 0; index < word.Length; index++)
        {
            var row = startRow + (index * rowDirection);
            var column = startColumn + (index * columnDirection);

            if (!IsWithinBounds(grid, row, column) || (grid[row, column] != '\0' && grid[row, column] != word[index]))
            {
                return false;
            }
        }

        return true;
    }

    // Logique de placement des mots dans la grille.
    private static bool PlaceWordInGrid(char[,] grid, string word)
    {
        var size = grid.GetLength(0);
        var cellCount = size * size;
        var startCell = Random.Shared.Next(cellCount);

        for (var i = 0; i < cellCount; i++)
        {
            var currentCell = (startCell + i) % cellCount;

            var row = currentCell / size;
            var column = currentCell % size;

            var startDirection = Random.Shared.Next(Directions.Length);

            for (var directionIndex = 0; directionIndex < Directions.Length; directionIndex++)
            {
                var currentDirection = (startDirection + directionIndex) % Directions.Length;
                var direction = Directions[currentDirection];

                if (CanPlaceWordAt(grid, word, row, column, direction.Row, direction.Column))
                {
                    for (var index = 0; index < word.Length; index++)
                    {
                        var newRow = row + (index * direction.Row);
                        var newColumn = column + (index * direction.Column);
                        grid[newRow, newColumn] = word[index];
                    }

                    return true;
                }
            }
        }

        return false;
    }

    // Compte le nombre d'occurrences d'un mot dans la grille.
    private static int CountWordOccurrences(char[,] grid, string word)
    {
        var count = 0;

        for (var row = 0; row < grid.GetLength(0); row++)
        {
            for (var column = 0; column < grid.GetLength(1); column++)
            {
                foreach (var direction in Directions)
                {
                    if (DoesWordMatchAt(grid, word, row, column, direction.Row, direction.Column))
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    // Convertit la grille de caractères en une liste de chaînes représentant les lignes.
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
