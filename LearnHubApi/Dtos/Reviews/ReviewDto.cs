namespace LearnHubApi.Dtos.Reviews;

public record ReviewDto(
    int Id,
    double Rating,
    string Comment,
    int AuthorId,
    string AuthorName,
    int CourseId,
    string CourseTitle,
    DateTime CreatedAt
);