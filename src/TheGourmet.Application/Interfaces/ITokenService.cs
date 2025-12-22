using System.Security.Claims;
using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Application.Interfaces;

public interface ITokenService
{
    (string, string) GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}