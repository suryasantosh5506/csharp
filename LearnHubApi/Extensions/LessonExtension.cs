using LearnHubApi.Dtos.Lessons;
using LearnHubApi.Entities;

namespace LearnHubApi.Extensions;

public static class LessonExtension
{
    public static LessonDto ToDto(this Lesson lesson)
    {
        return new LessonDto(lesson.Id,lesson.Title,lesson.Description,lesson.VideoUrl,lesson.PublicId,lesson.Duration,
                                lesson.Order,lesson.ModuleId,lesson.Module.Title);
    }
}