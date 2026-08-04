using JobPortal.Data;
using JobPortal.Dtos.Auth;
using JobPortal.Entities;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Services;

public class AuthService(AppDbContext context,PasswordHasher<User> passwordHasher,ITokenService tokenService) : IAuthService
{
    public async Task<LoginResponseDto> Login(LoginDto dto)
    {
        var user=await context.Users.FirstOrDefaultAsync(x=>x.Email.ToLower()==dto.Email.Trim().ToLower());
        if(user is null)
        {
            throw new NotFoundException("User Not Found");
        }
        if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid Credentials");
        }
        var token=tokenService.GenerateToken(user);
        return new LoginResponseDto(user.Id,token,user.Email,user.FullName,user.Role);
    }

    public async Task<LoginResponseDto> Register(RegisterDto dto)
    {
        if(await context.Users.AnyAsync(x => x.Email == dto.Email))
        {
            throw new ConflictException("Email alraedy registered");
        }
        User user=new User()
        {
            FullName=dto.FullName.Trim(),
            Email=dto.Email.Trim(),
            ProfileImageUrl=dto.ProfileImageUrl,
            PasswordHash=string.Empty,
            CreatedAt=DateTime.UtcNow
        };

        user.PasswordHash=passwordHasher.HashPassword(user,dto.Password);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var token=tokenService.GenerateToken(user);
        return new LoginResponseDto(user.Id,token,user.Email,user.FullName,user.Role);
    }
}