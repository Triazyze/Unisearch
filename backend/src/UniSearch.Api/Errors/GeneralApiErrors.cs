using UniSearch.Api.Models.Dtos;

namespace UniSearch.Api.Errors;

public static class GeneralApiErrors
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
}
