using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IJwtKeyProvider _jwtKeyProvider;
    public TokenService(IJwtKeyProvider jwtKeyProvider)
    {
        _jwtKeyProvider = jwtKeyProvider;
    }
    
    public string GenerateAccessToken(string userId, string role)
    {
        // Get private key from provider 
        var rsa = _jwtKeyProvider.GetPrivateKey();

        // Bọc vào RsaSecurityKey để JWT sử dụng
        var rsaSecurityKey = new RsaSecurityKey(rsa);

        var credentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

        // Nhét dữ liệu cần lưu vào token (claims)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        // Cấu hình chi tiết về token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = credentials,
            Issuer = "TheGourmet",
            Audience = "TheGourmetUser"
        };

        // Tạo token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        
        using var rng = RandomNumberGenerator.Create(); // using var để tự động giải phóng tài nguyên, khi dùng xong là dispose luôn ở cuối khối
        rng.GetBytes(randomNumber);
        
        return Convert.ToBase64String(randomNumber); // Trả về chuỗi base64 của mảng byte ngẫu nhiên
    }
}