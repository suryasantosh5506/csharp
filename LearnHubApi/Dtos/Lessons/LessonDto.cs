namespace LearnHubApi.Dtos.Lessons;

public record LessonDto(
    int Id,
    string Title,
    string Description,
    string VideoUrl,
    string PublicId,
    double Duration,
    int Order,
    int ModuleId,
    string ModuleTitle
);