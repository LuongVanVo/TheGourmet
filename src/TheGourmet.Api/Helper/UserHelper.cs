using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TheGourmet.Api.Helper;

public static class UserHelper
{
    public static Guid GetCurrentUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID not found in token");

        return Guid.Parse(userIdClaim);
    }
}