using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtAuth.Data;
using JwtAuth.Dtos;
using JwtAuth.Interfaces;
using JwtAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuth.Services;

public class AuthService(JwtAuthContext dbContext,IConfiguration configuration) : IAuthService
{
    public async Task<bool> RegisterAsync(RegisterUserDto userDto)
    {
        if(await dbContext.Users.AnyAsync(x => x.UserName == userDto.UserName))
        {
            return false;
        }
        var passwordHasher=new PasswordHasher<User>();
        User user = new()
        {
            UserName=userDto.UserName,
            PasswordHash=string.Empty
        };
        user.PasswordHash=passwordHasher.HashPassword(user,userDto.Password);
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginUserDto userDto)
    {
        var user=await dbContext.Users.FirstOrDefaultAsync(x=>x.UserName==userDto.UserName);
        if(user is null) return new(string.Empty);
        var passwordHasher=new PasswordHasher<User>();
        if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password) == PasswordVerificationResult.Failed)
        {
            return new(string.Empty);
        }
        return GenerateToken(user);
    }

    private LoginResponseDto GenerateToken(User user)
    {
        var claims=new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
            new Claim(ClaimTypes.Name,user.UserName)
        };

        var secret=configuration["JwtToken:SecretKey"]??"";
        var securityKey=new SymmetricSecurityKey(
           Encoding.UTF8.GetBytes(secret)
        );
        var credentials=new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["JwtToken:Issuer"],
            audience: configuration["JwtToken:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(configuration["JwtToken:ExpiresIn"])
            ),
            signingCredentials: credentials
        );
        var tokenString=new JwtSecurityTokenHandler().WriteToken(token);
        return new LoginResponseDto(tokenString);
    }
}