using LearnHubApi.Data;
using LearnHubApi.Dtos.Courses;
using LearnHubApi.Entities;
using LearnHubApi.Enums;
using LearnHubApi.Extensions;
using LearnHubApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnHubApi.Services;

public class CourseService(AppDbContext context, ICurrentUserService userService) : ICourseService
{
    public async Task<CourseDto> CreateAsync(CreateCourseDto dto)
    {
        if (!userService.IsAuthenticated)
            throw new Exception("Unauthorized");

        if (userService.Role != UserRole.Instructor &&
            userService.Role != UserRole.Admin)
        {
            throw new Exception("Only instructors and admins can create courses.");
        }

        if (!await context.Categories.AnyAsync(x => x.Id == dto.CategoryId))
        {
            throw new Exception("Category not found.");
        }

        if (await context.Courses.AnyAsync(x =>
            x.Title.ToLower() == dto.Title.Trim().ToLower()))
        {
            throw new Exception("Course already exists.");
        }

        Course course = new()
        {
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Thumbnail = dto.Thumbnail.Trim(),
            Price = dto.Price,
            Language = dto.Language.Trim(),
            Duration = dto.Duration,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InstructorId = userService.UserId,
            CategoryId = dto.CategoryId
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync();

        course = await context.Courses
            .Include(x => x.Instructor)
            .Include(x => x.Category)
            .FirstAsync(x => x.Id == course.Id);

        return course.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        if (!userService.IsAuthenticated)
            throw new Exception("Unauthorized");

        var course = await context.Courses
            .FirstOrDefaultAsync(x => x.Id == id);

        if (course is null)
            throw new Exception("Course not found.");

        if (userService.Role != UserRole.Admin &&
            course.InstructorId != userService.UserId)
        {
            throw new Exception("Forbidden");
        }

        if (await context.Enrollments.AnyAsync(x => x.CourseId == id))
        {
            throw new Exception("Cannot delete a course with active enrollments.");
        }

        context.Courses.Remove(course);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        return await context.Courses
            .Include(x => x.Instructor)
            .Include(x => x.Category)
            .Select(x => x.ToDto())
            .ToListAsync();
    }

    public async Task<CourseDto> GetByIdAsync(int id)
    {
        var course = await context.Courses
            .Include(x => x.Instructor)
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (course is null)
            throw new Exception("Course not found.");

        return course.ToDto();
    }

    public async Task<CourseDto> UpdateAsync(int id, UpdateCourseDto dto)
    {
        if (!userService.IsAuthenticated)
            throw new Exception("Unauthorized");

        if (userService.Role != UserRole.Instructor &&
            userService.Role != UserRole.Admin)
        {
            throw new Exception("Only instructors and admins can update courses.");
        }

        var course = await context.Courses
            .Include(x => x.Instructor)
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (course is null)
            throw new Exception("Course not found.");

        if (userService.Role != UserRole.Admin &&
            course.InstructorId != userService.UserId)
        {
            throw new Exception("Forbidden");
        }

        if (!await context.Categories.AnyAsync(x => x.Id == dto.CategoryId))
        {
            throw new Exception("Category not found.");
        }

        if (await context.Courses.AnyAsync(x =>
            x.Title.ToLower() == dto.Title.Trim().ToLower() &&
            x.Id != id))
        {
            throw new Exception("Course already exists.");
        }

        course.Title = dto.Title.Trim();
        course.Description = dto.Description.Trim();
        course.Thumbnail = dto.Thumbnail.Trim();
        course.Price = dto.Price;
        course.Language = dto.Language.Trim();
        course.Duration = dto.Duration;
        course.CategoryId = dto.CategoryId;
        course.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return course.ToDto();
    }
}