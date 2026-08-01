using LearnHubApi.Data;
using LearnHubApi.Dtos.Authorization;
using LearnHubApi.Entities;
using LearnHubApi.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearnHubApi.Services;

public class AuthService(AppDbContext context,ITokenService tokenService,PasswordHasher<User> passwordHasher) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user=await context.Users.FirstOrDefaultAsync(x=>x.Email==loginDto.Email);
        if(user is null)
        {
            throw new Exception("User not registered");
        }
        if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password) == PasswordVerificationResult.Failed)
        {
            throw new Exception("Invalid Credentials");
        }
        var token=tokenService.GenerateToken(user);
        return new LoginResponseDto(token,user.FirstName,user.LastName,user.Email,user.Role,user.Id);
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if(await context.Users.AnyAsync(x => x.Email == registerDto.Email))
        {
            throw new Exception("Email Already Registered");
        }

        User user=new User()
        {
            Email=registerDto.Email,
            FirstName=registerDto.FirstName,
            LastName=registerDto.LastName,
            PasswordHash="",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.PasswordHash=passwordHasher.HashPassword(user,registerDto.Password);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return await LoginAsync(new LoginDto(registerDto.Email,registerDto.Password));
    }
}