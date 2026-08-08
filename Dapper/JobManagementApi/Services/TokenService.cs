using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobManagementApi.Entities;
using JobManagementApi.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace JobManagementApi.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(User user)
    {
        List<Claim> claims=new()
        {
          new (ClaimTypes.NameIdentifier,user.Id.ToString()),
          new (ClaimTypes.Name,user.Name),
          new (ClaimTypes.Role,user.Role.ToString()),
          new (ClaimTypes.Email,user.Email),
        };

        var issuer=configuration["Jwt:Issuer"];
        var audience=configuration["Jwt:Audience"];
        var key=configuration["Jwt:Key"]??"";
        var expiresIn=int.Parse(configuration["Jwt:ExpirationMinutes"]??"0");

        var securityKey=new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key)
        );

        var signingCredentials=new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        var token=new JwtSecurityToken(
            issuer:issuer,
            audience:audience,
            signingCredentials:signingCredentials,
            expires:DateTime.UtcNow.AddMinutes(expiresIn),
            claims:claims
        );

        var tokenstring=new JwtSecurityTokenHandler().WriteToken(token);
        return tokenstring;
    }
}