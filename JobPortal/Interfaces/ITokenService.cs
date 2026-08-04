using JobPortal.Dtos.Auth;
using JobPortal.Entities;

namespace JobPortal.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}