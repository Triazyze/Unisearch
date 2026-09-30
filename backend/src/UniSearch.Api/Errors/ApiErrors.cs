using UniSearch.Api.Models.Dtos;

namespace UniSearch.Api.Errors;

public static class ApiErrors
{
    public static readonly ErrorResponse BadRequest = new (
        StatusCodes.Status400BadRequest,
        "BAD_REQUEST",
        "La requête est invalide.");

    public static readonly ErrorResponse Unauthorized = new (
        StatusCodes.Status401Unauthorized,
        "UNAUTHORIZED",
        "L'authentification a échoué.");

    public static readonly ErrorResponse Forbidden = new (
        StatusCodes.Status403Forbidden,
        "FORBIDDEN",
        "Vous n'avez pas les droits nécessaires pour accéder à cette ressource.");

    public static readonly ErrorResponse TooManyRequests = new (
        StatusCodes.Status429TooManyRequests,
        "TOO_MANY_REQUESTS",
        "Trop de requêtes ont été envoyées. Réessayez plus tard.");

    public static readonly ErrorResponse UserAlreadyExists = new (
        StatusCodes.Status409Conflict,
        "USER_ALREADY_EXISTS",
        "Un utilisateur utilise déjà ce nom ou cette adresse email.");

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
}
