using JobManagementApi.Dtos.Skills;
using JobManagementApi.Entities;
using JobManagementApi.RequestHelpers.Pagination;

namespace JobManagementApi.Interfaces;

public interface ISkillService
{
    Task<PagedList<SkillDto>> GetAllSkillsAsync(PaginationParams paginationParams);
    Task<SkillDto> GetSkillAsync(int id);
    Task<SkillDto> CreateSkillAsync(CreateSkillDto dto);
    Task<bool> UpdateSkillAsync(int id,UpdateSkillDto dto);
    Task<bool> DeleteSkillAsync(int id);
}