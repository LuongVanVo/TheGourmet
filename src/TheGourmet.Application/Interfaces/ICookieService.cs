namespace TheGourmet.Application.Interfaces
{
    public interface ICookieService
    {
        void SetAuthCookies(string accessToken, string refreshToken);
        void RemoveAuthCookies();
    }
}