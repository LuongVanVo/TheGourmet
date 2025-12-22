using TheGourmet.Application.DTOs.Auth;

namespace TheGourmet.Application.Interfaces;

public interface IAuthService
{
    public Task<AuthResponse> RegisterAsync(RegisterRequest request);
    public Task<AuthResponse> LoginAsync(LoginRequest request);
    public Task<bool> ConfirmEmailAsync(string userId, string token);
    public Task<AuthResponse> RefreshTokenAsync(string token);
    public Task<AuthResponse> LogoutAsync(string refreshToken);
}