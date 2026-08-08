using JobManagementApi.Dtos.Auth;
using JobManagementApi.Entities;

namespace JobManagementApi.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> Register(RegisterDto dto);
    Task<LoginResponseDto> Login(LoginDto dto);
}