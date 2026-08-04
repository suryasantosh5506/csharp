using JobPortal.Dtos.Auth;

namespace JobPortal.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> Register(RegisterDto dto);
    Task<LoginResponseDto> Login(LoginDto dto);
}