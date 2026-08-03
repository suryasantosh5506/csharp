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

public class CourseService(AppDbContext context, ICurrentUserService userService,ICloudinaryService cloudinaryService) : ICourseService
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

        var result=await cloudinaryService.ImageUploadAsync(dto.Thumbnail);

        Course course = new()
        {
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Thumbnail = result.SecureUrl.AbsoluteUri,
            PublicId=result.PublicId,
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

        await cloudinaryService.DeleteImageAsync(course.PublicId);

        context.Courses.Remove(course);
        await context.SaveChangesAsync();
    }

    public async Task<PagedList<CourseDto>> GetAllAsync(CourseParams courseParams)
    {
        var query= context.Courses
        .Where(x=>string.IsNullOrEmpty(courseParams.Search) 
                        || x.Title.ToLower().Contains(courseParams.Search.ToLower()) 
                        || x.Description.ToLower().Contains(courseParams.Search.ToLower()))
                    .Include(x => x.Instructor)
                    .Include(x => x.Category)
                    .Select(x => x.ToDto());
        
        if(courseParams.CategoryId.HasValue)
            query=query.Where(x=>x.CategoryId==courseParams.CategoryId);

        if(!string.IsNullOrEmpty(courseParams.Language)) query=query.Where(x=>x.Language.ToLower()==courseParams.Language.ToLower());

        if(courseParams.MinPrice.HasValue) query=query.Where(x=>x.Price>=courseParams.MinPrice);
        if(courseParams.MaxPrice.HasValue) query=query.Where(x=>x.Price<=courseParams.MaxPrice);
        if(courseParams.MinDuration.HasValue) query=query.Where(x=>x.Duration>=courseParams.MinDuration);
        if(courseParams.MaxDuration.HasValue) query=query.Where(x=>x.Duration<=courseParams.MaxDuration);
        query=courseParams.SortBy?.ToLower() switch
        {
            "price"=>query.OrderBy(x=>x.Price),
            "price_desc"=>query.OrderByDescending(x=>x.Price),
            "title"=>query.OrderBy(x=>x.Title),
            "duration"=>query.OrderBy(x=>x.Duration),
            _=>query.OrderBy(x=>x.Title),
        };
        var response=await PagedList<CourseDto>.ToPagedList(query,courseParams.PageNumber,courseParams.PageSize);
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

        if(dto.Thumbnail is not null && dto.Thumbnail.Length > 0)
        {
            await cloudinaryService.DeleteImageAsync(course.PublicId);
            var result=await cloudinaryService.ImageUploadAsync(dto.Thumbnail);
            course.Thumbnail=result.SecureUrl.AbsoluteUri;
            course.PublicId=result.PublicId;
        }

        course.Title = dto.Title.Trim();
        course.Description = dto.Description.Trim();
        course.Price = dto.Price;
        course.Language = dto.Language.Trim();
        course.Duration = dto.Duration;
        course.CategoryId = dto.CategoryId;
        course.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return course.ToDto();
    }
}