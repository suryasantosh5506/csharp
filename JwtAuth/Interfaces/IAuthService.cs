using JwtAuth.Dtos;

namespace JwtAuth.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterUserDto userDto);
    Task<LoginResponseDto> LoginAsync(LoginUserDto userDto);
}