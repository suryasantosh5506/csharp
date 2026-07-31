using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.User;
using HospitalManagementAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

public class AuthController(
    HospitalContext context,
    IAuthService authService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterDto newUser)
    {
        if(await context.Users.AnyAsync(x=>
            x.Email.ToLower()==newUser.Email.Trim().ToLower()))
        {
            return Conflict("Email is already registered.");
        }

        await authService.RegisterAsync(newUser);

        return Ok(new {message="User registered successfully."});
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        var response=await authService.LoginAsync(loginDto);

        if(response is null)
            return Unauthorized("Invalid email or password.");

        return Ok(response);
    }
}