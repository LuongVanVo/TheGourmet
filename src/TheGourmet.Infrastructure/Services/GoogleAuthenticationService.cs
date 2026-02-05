using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TheGourmet.Application.DTOs.Auth;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Infrastructure.Services;

public class GoogleAuthenticationService : IGoogleAuthenticationService
{
    private readonly IConfiguration _configuration;
    public GoogleAuthenticationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<GoogleUserInfoDto> ValidateTokenAndGetUserInfoAsync(string authCode)
    {
        var clientId = _configuration["GOOGLE_CLIENT_ID"];
        var clientSecret = _configuration["GOOGLE_CLIENT_SECRET"];
        var redirectUri = _configuration["GOOGLE_REDIRECT_URI"];

        // Exchange auth code for token
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");
        request.Content = new FormUrlEncodedContent(new []
        {
            new KeyValuePair<string, string>("code", authCode),
            new KeyValuePair<string, string>("client_id", clientId ?? string.Empty),
            new KeyValuePair<string, string>("client_secret", clientSecret ?? string.Empty),
            new KeyValuePair<string, string>("redirect_uri", redirectUri ?? string.Empty),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        });

        var response = await client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode) throw new Exception("Lỗi xác thực với Google.");

        var tokenData = JsonConvert.DeserializeObject<dynamic>(responseString);
        string accessToken = tokenData.access_token;

        // Get user info
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var infoResponse = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
        var infoString = await infoResponse.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<GoogleUserInfoDto>(infoString);
    }
}