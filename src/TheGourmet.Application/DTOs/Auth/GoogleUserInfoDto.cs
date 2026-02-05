namespace TheGourmet.Application.DTOs.Auth;

public class GoogleUserInfoDto
{
    public string Id { get; set; } = string.Empty; // Google User ID
    public string Email { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public string Name { get; set; } = string.Empty; // Full name
    public string Picture { get; set; } = string.Empty; // URL to profile picture
}