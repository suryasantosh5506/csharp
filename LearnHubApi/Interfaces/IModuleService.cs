using LearnHubApi.Dtos.Modules;

namespace LearnHubApi.Interfaces;

public interface IModuleService
{
    Task<IEnumerable<ModuleDto>> GetAllAsync();

    Task<ModuleDto> GetByIdAsync(int id);

    Task<ModuleDto> CreateAsync(CreateModuleDto dto);

    Task<ModuleDto> UpdateAsync(int id, UpdateModuleDto dto);

    Task DeleteAsync(int id);
}