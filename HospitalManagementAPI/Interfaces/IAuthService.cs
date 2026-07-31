using HospitalManagementAPI.Dtos.User;

namespace HospitalManagementAPI.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto newUser);

    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
}