using LearnHubApi.Dtos.Courses;
using LearnHubApi.RequestHelpers;

namespace LearnHubApi.Interfaces;

public interface ICourseService
{
    Task<PagedList<CourseDto>> GetAllAsync(PaginationParams paginationParams);

    Task<CourseDto> GetByIdAsync(int id);

    Task<CourseDto> CreateAsync(CreateCourseDto dto);

    Task<CourseDto> UpdateAsync(int id, UpdateCourseDto dto);

    Task DeleteAsync(int id);
}