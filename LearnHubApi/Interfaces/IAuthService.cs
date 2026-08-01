using LearnHubApi.Dtos.Authorization;

namespace LearnHubApi.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
}