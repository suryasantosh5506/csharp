using LearnHubApi.Dtos.Authorization;
using LearnHubApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LearnHubApi.Controllers;

public class AuthController(IAuthService authService) : BaseApiController
{
    [HttpPost("/register")]
    public async Task<ActionResult<LoginResponseDto>> RegisterAsync(RegisterDto dto)
    {
        return Ok(await authService.RegisterAsync(dto));
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> LoginAsync(LoginDto dto)
    {
        return Ok(await authService.LoginAsync(dto));
    }
}