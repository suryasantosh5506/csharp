using LearnHubApi.Data;
using LearnHubApi.Dtos.Courses;
using LearnHubApi.Entities;
using LearnHubApi.Enums;
using LearnHubApi.Exceptions;
using LearnHubApi.Extensions;
using LearnHubApi.Interfaces;
using LearnHubApi.RequestHelpers;
using Microsoft.EntityFrameworkCore;

namespace LearnHubApi.Services;

public class CourseService(AppDbContext context, ICurrentUserService userService) : ICourseService
{
    public async Task<CourseDto> CreateAsync(CreateCourseDto dto)
    {
        if (!userService.IsAuthenticated)
            throw new UnauthorizedException("Unauthorized");

        if (userService.Role != UserRole.Instructor && userService.Role != UserRole.Admin)
        {
            throw new ForbiddenException("Only instructors and admins can create courses.");
        }

        if (!await context.Categories.AnyAsync(x => x.Id == dto.CategoryId))
        {
            throw new NotFoundException("Category not found.");
        }

        if (await context.Courses.AnyAsync(x =>
            x.Title.ToLower() == dto.Title.Trim().ToLower()))
        {
            throw new ConflictException("Course already exists.");
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

        course = await context.Courses.Include(x => x.Instructor).Include(x => x.Category).FirstAsync(x => x.Id == course.Id);

        return course.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        if (!userService.IsAuthenticated)
            throw new UnauthorizedException("Unauthorized");

        var course = await context.Courses.FirstOrDefaultAsync(x => x.Id == id);

        if (course is null)
            throw new NotFoundException("Course not found.");

        if (userService.Role != UserRole.Admin && course.InstructorId != userService.UserId)
        {
            throw new ForbiddenException("Forbidden");
        }

        if (await context.Enrollments.AnyAsync(x => x.CourseId == id))
        {
            throw new ConflictException("Cannot delete a course with active enrollments.");
        }

        context.Courses.Remove(course);
        await context.SaveChangesAsync();
    }

    public async Task<PagedList<CourseDto>> GetAllAsync(PaginationParams paginationParams)
    {
        var query= context.Courses.Include(x => x.Instructor).Include(x => x.Category).Select(x => x.ToDto());
        var response=await PagedList<CourseDto>.ToPagedList(query,paginationParams.PageNumber,paginationParams.PageSize);
        return response;
    }

    public async Task<CourseDto> GetByIdAsync(int id)
    {
        var course = await context.Courses.Include(x => x.Instructor).Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);

        if (course is null)
            throw new NotFoundException("Course not found.");

        return course.ToDto();
    }

    public async Task<CourseDto> UpdateAsync(int id, UpdateCourseDto dto)
    {
        if (!userService.IsAuthenticated)
            throw new UnauthorizedException("Unauthorized");

        if (userService.Role != UserRole.Instructor && userService.Role != UserRole.Admin)
        {
            throw new ForbiddenException("Only instructors and admins can update courses.");
        }

        var course = await context.Courses.Include(x => x.Instructor).Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);

        if (course is null)
            throw new NotFoundException("Course not found.");

        if (userService.Role != UserRole.Admin &&course.InstructorId != userService.UserId)
        {
            throw new ForbiddenException("Forbidden");
        }

        if (!await context.Categories.AnyAsync(x => x.Id == dto.CategoryId))
        {
            throw new NotFoundException("Category not found.");
        }

        if (await context.Courses.AnyAsync(x =>x.Title.ToLower() == dto.Title.Trim().ToLower() && x.Id != id))
        {
            throw new ConflictException("Course already exists.");
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