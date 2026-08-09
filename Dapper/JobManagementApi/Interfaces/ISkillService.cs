using JobManagementApi.Dtos.Skills;
using JobManagementApi.Entities;

namespace JobManagementApi.Interfaces;

public interface ISkillService
{
    Task<IEnumerable<SkillDto>> GetAllSkillsAsync();
    Task<SkillDto> GetSkillAsync(int id);
    Task<SkillDto> CreateSkillAsync(CreateSkillDto dto);
    Task<bool> UpdateSkillAsync(int id,UpdateSkillDto dto);
    Task<bool> DeleteSkillAsync(int id);
}