using LearnHubApi.Dtos.Lessons;

namespace LearnHubApi.Interfaces;

public interface ILessonService
{
    Task<IEnumerable<LessonDto>> GetByModuleAsync(int moduleId);

    Task<LessonDto> GetByIdAsync(int id);

    Task<LessonDto> CreateAsync(CreateLessonDto dto);

    Task<LessonDto> UpdateAsync(int id, UpdateLessonDto dto);

    Task DeleteAsync(int id);
}