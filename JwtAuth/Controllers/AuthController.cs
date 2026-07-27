using JwtAuth.Dtos;
using JwtAuth.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuth.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService):ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterUserDto userDto)
    {
        var result=await authService.RegisterAsync(userDto);
        if (result == false)
        {
            
            return BadRequest("Username already exists");
        }
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> LoginAsync(LoginUserDto userDto)
    {
        var response=await authService.LoginAsync(userDto);
        if(response.Token==string.Empty) return BadRequest("Usename or Password is invalid");
        return Ok(response);
    }
};