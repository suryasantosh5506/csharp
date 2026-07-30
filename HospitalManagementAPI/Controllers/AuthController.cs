using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.User;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HospitalManagementAPI.Controllers;

public class AuthController(
    HospitalContext context,
    PasswordHasher<User> passwordHasher,
    IOptions<JwtConfiguration> jwtOptions) : BaseApiController
{
    private readonly JwtConfiguration jwtConfiguration = jwtOptions.Value;

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterDto newUser)
    {
        if (await context.Users.AnyAsync(x =>
            x.Email.ToLower() == newUser.Email.Trim().ToLower()))
        {
            return Conflict("Email is already registered.");
        }

        var user = new User
        {
            FirstName = newUser.FirstName.Trim(),
            LastName = newUser.LastName.Trim(),
            Email = newUser.Email.Trim(),
            PasswordHash = "",
            Role = "Patient",
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = passwordHasher.HashPassword(user, newUser.Password);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Ok(new { message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(x =>
            x.Email.ToLower() == loginDto.Email.Trim().ToLower());

        if (user is null)
            return Unauthorized("Invalid email or password.");

        var result = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            loginDto.Password);

        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid email or password.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName)
        };

        var secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey));

        var signingCredentials = new SigningCredentials(
            secretKey,
            SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: jwtConfiguration.Issuer,
            audience: jwtConfiguration.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return Ok(new LoginResponseDto(token));
    }
}