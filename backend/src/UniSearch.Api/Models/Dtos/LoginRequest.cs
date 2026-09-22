using System.ComponentModel.DataAnnotations;

namespace UniSearch.Api.Models.Dtos;

public sealed class LoginRequest
{
    [Required]
    public string Identifier { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
