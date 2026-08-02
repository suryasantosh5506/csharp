namespace LearnHubApi.Dtos.Modules;

public record ModuleDto(
    int Id,
    string Title,
    string Description,
    int Order,
    int CourseId,
    string CourseName,
    int LessonsCount
);