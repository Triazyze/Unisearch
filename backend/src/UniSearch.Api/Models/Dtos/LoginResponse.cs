using System.ComponentModel.DataAnnotations;

namespace UniSearch.Api.Models.Dtos;

public sealed class LoginResponse
{
    public string Token { get; init; } = string.Empty;
}
