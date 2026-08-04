using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JobPortal.Dtos.Auth;
using JobPortal.Entities;
using JobPortal.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace JobPortal.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(User user)
    {

        string issuer = configuration["Jwt:Issuer"] ?? "";
        string audience = configuration["Jwt:Audience"] ?? "";
        int expiresIn = int.Parse(configuration["Jwt:ExpireMinutes"] ?? "60");
        string key = configuration["Jwt:Key"] ?? "";

        var claims=new List<Claim>{
            new(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new(ClaimTypes.Name,user.FullName),
            new(ClaimTypes.Email,user.Email),
            new(ClaimTypes.Role,user.Role.ToString())
        };

        var securityKey=new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key)
        );

        var credentials=new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);


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