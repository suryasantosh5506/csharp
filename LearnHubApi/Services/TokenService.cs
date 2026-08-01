using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LearnHubApi.Entities;
using LearnHubApi.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace LearnHubApi.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(User user)
    {
        string issuer = configuration["Jwt:Issuer"] ?? "";
        string audience = configuration["Jwt:Audience"] ?? "";
        int expiresIn = int.Parse(configuration["Jwt:ExpireMinutes"] ?? "60");
        string key = configuration["Jwt:Key"] ?? "";

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key)
        );

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var jwtToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresIn),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}