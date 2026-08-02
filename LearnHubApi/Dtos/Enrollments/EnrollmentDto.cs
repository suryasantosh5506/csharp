namespace LearnHubApi.Dtos.Enrollments;

public record EnrollmentDto(
    int Id,
    int StudentId,
    string StudentName,
    int CourseId,
    string CourseName,
    DateTime EnrolledAt
);