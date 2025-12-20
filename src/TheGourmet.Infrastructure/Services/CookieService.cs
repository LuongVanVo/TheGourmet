using Microsoft.AspNetCore.Http;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Infrastructure.Services
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetAuthCookies(string accessToken, string refreshToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            var accessTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };

            var refreshTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            // Set cookies to response
            context.Response.Cookies.Append("access_token", accessToken, accessTokenCookieOptions);

            if (!string.IsNullOrEmpty(refreshToken))
            {
                context.Response.Cookies.Append("refresh_token", refreshToken, refreshTokenCookieOptions);
            }
        }

        public void RemoveAuthCookies()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            context.Response.Cookies.Delete("access_token");
            context.Response.Cookies.Delete("refresh_token");
        }
    }
}