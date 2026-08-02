using LearnHubApi.Dtos.Enrollments;
using LearnHubApi.Entities;

namespace LearnHubApi.Extensions;

public static class EnrollmentExtension
{
    public static EnrollmentDto ToDto(this Enrollment enrollment)
    {
        return new EnrollmentDto(enrollment.Id,enrollment.StudentId,enrollment.Student.FirstName+enrollment.Student.LastName,
                                    enrollment.CourseId,enrollment.Course.Title,enrollment.EnrolledAt);
    }
}