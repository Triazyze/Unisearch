using System.ComponentModel.DataAnnotations;

namespace UniSearch.Api.Models.Dtos.Authentication;

public sealed class RegisterRequest
{
    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(12)]
    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$",
        ErrorMessage = "Le mot de passe doit contenir au moins une lettre majuscule, un chiffre et un caractère spécial.")]
    public string Password { get; init; } = string.Empty;
}
