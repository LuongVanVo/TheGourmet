using System.Text.Json.Serialization;

namespace TheGourmet.Application.Features.Auth.Results;

public class AuthResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string? AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; } = string.Empty;
}