using LearnHubApi.Dtos.Category;
using LearnHubApi.RequestHelpers;

namespace LearnHubApi.Interfaces;

public interface ICategoryService
{
    Task<PagedList<CategoryDto>> GetAllAsync(PaginationParams paginationParams);

    Task<CategoryDto> GetByIdAsync(int id);

    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);

    Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto);

    Task DeleteAsync(int id);
}