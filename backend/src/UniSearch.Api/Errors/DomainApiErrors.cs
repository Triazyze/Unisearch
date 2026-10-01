using UniSearch.Api.Models.Dtos;

namespace UniSearch.Api.Errors;

public static class DomainApiErrors
{
    // Erreur auth
    public static readonly ErrorResponse UserAlreadyExists = new (
        StatusCodes.Status409Conflict,
        "USER_ALREADY_EXISTS",
        "Un utilisateur utilise déjà ce nom ou cette adresse email.");

    // Erreurs de vérification de grille
    public static readonly ErrorResponse GridNotFound = new (
        StatusCodes.Status404NotFound,
        "GRID_NOT_FOUND",
        "La grille demandée est introuvable.");

    public static readonly ErrorResponse TestPackNotFound = new (
        StatusCodes.Status404NotFound,
        "TEST_PACK_NOT_FOUND",
        "Le pack de test est introuvable.");

    public static readonly ErrorResponse LettersInvalid = new (
        StatusCodes.Status422UnprocessableEntity,
        "LETTERS_INVALID",
        "Les lettres soumises ne correspondent pas à la grille.");

    public static readonly ErrorResponse WordsInvalid = new (
        StatusCodes.Status422UnprocessableEntity,
        "WORDS_INVALID",
        "Les mots soumis ne correspondent pas à la grille.");

    public static readonly ErrorResponse TraceMismatch = new (
        StatusCodes.Status422UnprocessableEntity,
        "TRACE_MISMATCH",
        "Les tracés soumis ne correspondent pas aux mots.");

    public static readonly ErrorResponse TracesInvalid = new (
        StatusCodes.Status422UnprocessableEntity,
        "TRACES_INVALID",
        "Les tracés soumis sont invalides.");

    // Erreurs de création de grille
    public static readonly ErrorResponse MissingDifficulty = new (
        StatusCodes.Status422UnprocessableEntity,
        "MISSING_DIFFICULTY",
        "La difficulté est manquante.");

    public static readonly ErrorResponse IncorrectGridSize = new (
        StatusCodes.Status422UnprocessableEntity,
        "INCORRECT_GRID_SIZE",
        "La taille de la grille doit être comprise entre 4 et 10.");

    public static readonly ErrorResponse InvalidWords = new (
        StatusCodes.Status422UnprocessableEntity,
        "INVALID_WORDS",
        "Les mots doivent faire au moins 3 caractères et ne doivent pas être des palindromes");

    public static readonly ErrorResponse DuplicateWords = new (
        StatusCodes.Status422UnprocessableEntity,
        "DUPLICATE_WORDS",
        "Chaque mot doit être unique dans la liste des mots fournie.");

    public static readonly ErrorResponse NotEnoughWords = new (
        StatusCodes.Status422UnprocessableEntity,
        "NOT_ENOUGH_WORDS",
        "Entrez au moins un mot pour créer une grille.");

    public static readonly ErrorResponse PlacementFailed = new (
        StatusCodes.Status422UnprocessableEntity,
        "PLACEMENT_FAILED",
        "Le placement des mots a échoué.");
}
