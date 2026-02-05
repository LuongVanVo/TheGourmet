using TheGourmet.Application.DTOs.Auth;

namespace TheGourmet.Application.Interfaces;

public interface IGoogleAuthenticationService
{
    // Validate the Google auth code and return user info
    Task<GoogleUserInfoDto> ValidateTokenAndGetUserInfoAsync(string authCode);
}