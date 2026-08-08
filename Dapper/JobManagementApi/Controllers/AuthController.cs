using JobManagementApi.Dtos.Auth;
using JobManagementApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementApi.Controllers;

public class AuthController(IAuthService authService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<LoginResponseDto>> Register(RegisterDto dto)
    {
        var response = await authService.Register(dto);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        var response = await authService.Login(dto);

        return Ok(response);
    }
}