using LearnHubApi.Entities;

namespace LearnHubApi.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}