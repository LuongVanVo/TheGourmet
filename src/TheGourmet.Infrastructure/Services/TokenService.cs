using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using TheGourmet.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using TheGourmet.Domain.Entities.Identity;

namespace TheGourmet.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IJwtKeyProvider _rsaKeyProvider;
    public TokenService(IConfiguration configuration, IJwtKeyProvider rsaKeyProvider)
    {
        _rsaKeyProvider = rsaKeyProvider;
        _configuration = configuration;
    }
    
    public (string, string) GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        // 1. Lấy Private key từ file .pem
        var key = _rsaKeyProvider.GetPrivateKey();

        // 2. Tạo thuật toán ký số với RSA SHA256
        var credentials = new SigningCredentials(new RsaSecurityKey(key), SecurityAlgorithms.RsaSha256);

        // Sinh ID 
        var jwtId = Guid.NewGuid().ToString();
        // 3. Tạo Claims cho token (thông tin lưu trong token)
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, jwtId),
        };

        // Thêm role vào claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // 4. Cấu hình token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "30")), // Token hết hạn sau 30 phút
            SigningCredentials = credentials,
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        var tokenString = tokenHandler.WriteToken(token);
        return (tokenString, jwtId);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        
        using var rng = RandomNumberGenerator.Create(); // using var để tự động giải phóng tài nguyên, khi dùng xong là dispose luôn ở cuối khối
        rng.GetBytes(randomNumber);
        
        return Convert.ToBase64String(randomNumber); // Trả về chuỗi base64 của mảng byte ngẫu nhiên
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        // Validate token bằng Public key
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(_rsaKeyProvider.GetPublicKey()),
            ValidateLifetime = false // Token hết hạn vẫn cho đọc để lấy info refresh token
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        // check xem có đúng thuật toán RSA không
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}