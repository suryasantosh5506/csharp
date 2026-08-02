using LearnHubApi.Data;
using LearnHubApi.Dtos.Enrollments;
using LearnHubApi.Entities;
using LearnHubApi.Extensions;
using LearnHubApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnHubApi.Services;

public class EnrollmentService(AppDbContext context,ICurrentUserService userService) : IEnrollmentService
{
    public async Task DeleteAsync(int courseId)
    {
        if (!userService.IsAuthenticated)
        {
            throw new Exception("Unauthorized");
        }
        var course=await context.Courses.FindAsync(courseId);
        if(course is null)
        {
            throw new Exception("Course Not Found");
        }
        var enrollment=await context.Enrollments.FirstOrDefaultAsync(x=>x.StudentId==userService.UserId && x.CourseId == courseId);

        if(enrollment is null)
        {
            throw new Exception("You are not enrolled in this course.");
        }
        
        context.Enrollments.Remove(enrollment);
        await context.SaveChangesAsync();
    }

    public async Task<EnrollmentDto> EnrollAsync(CreateEnrollmentDto dto)
    {
        if (!userService.IsAuthenticated)
        {
            throw new Exception("Unauthorized");
        }
        var course=await context.Courses.FindAsync(dto.CourseId);
        if(course is null)
        {
            throw new Exception("Course Not Found");
        }
        if (course.InstructorId == userService.UserId)
        {
            throw new Exception("Instructor cannot Enroll to his own course");
        }

        if(await context.Enrollments.AnyAsync(x=>x.StudentId==userService.UserId && x.CourseId == dto.CourseId))
        {
            throw new Exception("You already enrolled to this course");
        }
        Enrollment enrollment=new Enrollment()
        {
            StudentId=userService.UserId,
            CourseId=dto.CourseId,
            EnrolledAt=DateTime.UtcNow
        };
        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync();
        enrollment=await context.Enrollments.Include(x=>x.Course).Include(x=>x.Student).FirstAsync(x=>x.Id==enrollment.Id);
        return enrollment.ToDto();
    }

    public async Task<IEnumerable<EnrollmentDto>> GetMyEnrollmentsAsync()
    {
        if (!userService.IsAuthenticated)
        {
            throw new Exception("Unauthorized");
        }
        return await context.Enrollments.Where(x=>x.StudentId==userService.UserId).Include(x=>x.Course).Include(x=>x.Student).Select(x=>x.ToDto()).OrderByDescending(x => x.EnrolledAt).ToListAsync();
    }
}