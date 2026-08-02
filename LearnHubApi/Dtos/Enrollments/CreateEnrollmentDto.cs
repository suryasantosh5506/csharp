using System.ComponentModel.DataAnnotations;

namespace LearnHubApi.Dtos.Enrollments;

public record CreateEnrollmentDto(
    [Range(1, int.MaxValue)]
    int CourseId
);