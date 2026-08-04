using JobPortal.Dtos.Auth;
using JobPortal.Interfaces;
using LearnHubApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Controllers;

public class AuthController(IAuthService authService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<LoginResponseDto>> Register(RegisterDto dto)
    {
        return Ok(await authService.Register(dto));
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        return Ok(await authService.Login(dto));
    }
}