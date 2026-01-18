using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MiniMazErpBack;

public class JwtService : IJwtService
{
    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ?? "fallback_key_32_chars_long_123456");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
            ]),

            Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(Environment.GetEnvironmentVariable("JWT_EXPIRE_HOURS") ?? "24")),
            Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "MiniMazErpBack",
            Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "MiniMazErpFront",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
