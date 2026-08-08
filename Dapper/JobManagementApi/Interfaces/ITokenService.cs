using JobManagementApi.Entities;

namespace JobManagementApi.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}