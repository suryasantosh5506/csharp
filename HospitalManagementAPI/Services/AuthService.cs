using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.User;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HospitalManagementAPI.Services;

public class AuthService(
    HospitalContext context,
    PasswordHasher<User> passwordHasher,
    IOptions<JwtConfiguration> jwtOptions) : IAuthService
{
    private readonly JwtConfiguration jwtConfiguration=jwtOptions.Value;

    public async Task RegisterAsync(RegisterDto newUser)
    {
        var user=new User
        {
            FirstName=newUser.FirstName.Trim(),
            LastName=newUser.LastName.Trim(),
            Email=newUser.Email.Trim(),
            PasswordHash="",
            Role="Patient",
            CreatedAt=DateTime.UtcNow
        };

        user.PasswordHash=passwordHasher.HashPassword(user,newUser.Password);

        context.Users.Add(user);

        await context.SaveChangesAsync();
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user=await context.Users.FirstOrDefaultAsync(x=>
            x.Email.ToLower()==loginDto.Email.Trim().ToLower());

        if(user is null)
            return null;

        var result=passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            loginDto.Password);

        if(result==PasswordVerificationResult.Failed)
            return null;

        var claims=new List<Claim>
        {
            new(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new(ClaimTypes.Email,user.Email),
            new(ClaimTypes.Role,user.Role),
            new(ClaimTypes.GivenName,user.FirstName),
            new(ClaimTypes.Surname,user.LastName)
        };

        var secretKey=new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey));

        var signingCredentials=new SigningCredentials(
            secretKey,
            SecurityAlgorithms.HmacSha256);

        var jwtToken=new JwtSecurityToken(
            issuer:jwtConfiguration.Issuer,
            audience:jwtConfiguration.Audience,
            claims:claims,
            expires:DateTime.UtcNow.AddHours(1),
            signingCredentials:signingCredentials);

        var token=new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return new LoginResponseDto(token);
    }
}