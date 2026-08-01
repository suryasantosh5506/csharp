using LearnHubApi.Dtos.Courses;
using LearnHubApi.Entities;

namespace LearnHubApi.Extensions;

public static class CourseExtension
{
    public static CourseDto ToDto(this Course course)
    {
        return new CourseDto(
            course.Id,
            course.Title,
            course.Description,
            course.Thumbnail,
            course.Price,
            course.Language,
            course.Duration,
            course.CategoryId,
            course.Category.Name,
            course.InstructorId,
            course.Instructor.FirstName+course.Instructor.FirstName
        );
    }
}