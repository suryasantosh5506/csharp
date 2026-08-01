namespace LearnHubApi.Dtos.Courses;

public record CourseDto(
    int Id,
    string Title,
    string  Description,
    string Thumbnail,
    decimal Price,
    string Language,
    double Duration,
    int CategoryId,
    string CategoryName,
    int InstructorId,
    string InstructorName
);