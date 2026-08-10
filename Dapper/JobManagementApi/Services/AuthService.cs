using System.Data;
using Dapper;
using JobManagementApi.Data;
using JobManagementApi.Dtos.Auth;
using JobManagementApi.Entities;
using JobManagementApi.Exceptions;
using JobManagementApi.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace JobManagementApi.Services;

public class AuthService(DapperContext context,PasswordHasher<User> passwordHasher,ITokenService tokenService,ILogger<AuthService>logger) : IAuthService
{
    public async Task<LoginResponseDto> Login(LoginDto dto)
    {
        using var connection=context.GetConnection();
        var query="Select * from User where email=@email";
        User? user = await connection.QueryFirstOrDefaultAsync<User?>(query, new { email = dto.Email });
        if(user is null)
        {
            logger.LogWarning("Login failed: user not found");
            throw new NotFoundException("User Not Found");
        }
        if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
        {
            logger.LogWarning("Login failed: incorrect credentials");
            throw new BadRequestException("Credentials Incorrect");
        }
        string token=tokenService.GenerateToken(user);
        logger.LogInformation($"User {user.Id} logged in successfully");
        return new LoginResponseDto(user.Id,user.Name,user.Email,user.Role,token);
    }

    public async Task<LoginResponseDto> Register(RegisterDto dto)
    {
        using var connection=context.GetConnection();
        
        User? user = await connection.QueryFirstOrDefaultAsync<User?>("GetUserByEmail", new { p_Email = dto.Email },commandType:CommandType.StoredProcedure);
        if(user is not null)
        {
            logger.LogWarning("Someone tried to register using already registered email");
            throw new ConflictException("User Already Exists");
        }
        User newUser=new User()
        {
            Name=dto.Name,
            Email=dto.Email,
            PasswordHash=string.Empty,
            Role=Enums.UserRole.Candidate
        };
        newUser.PasswordHash=passwordHasher.HashPassword(newUser,dto.Password);

        var parameters =new
        {
            p_Name=dto.Name,
            p_Email=dto.Email,
            p_PasswordHash=newUser.PasswordHash,
            p_Role=newUser.Role.ToString()
        };

        int rowsaffected=await connection.ExecuteAsync("InsertUser",parameters,commandType:CommandType.StoredProcedure);

        if (rowsaffected == 0)
        {
            logger.LogCritical("User registration failed 0 rows affected in the database");
            throw new Exception("Internal Server Error");
        }
        
        logger.LogInformation("User registered successfully");
        user = await connection.QueryFirstAsync<User>("GetUserByEmail", new { p_Email = dto.Email },commandType:CommandType.StoredProcedure);
        string token=tokenService.GenerateToken(user);
        logger.LogInformation($"User {user.Id} logged in successfully");
        return new LoginResponseDto(user.Id,user.Name,user.Email,user.Role,token);
    }
}