using LearnHubApi.Data;
using LearnHubApi.Dtos.Lessons;
using LearnHubApi.Entities;
using LearnHubApi.Enums;
using LearnHubApi.Extensions;
using LearnHubApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace LearnHubApi.Services;

public class LessonService(AppDbContext context,ICurrentUserService userService,ICloudinaryService cloudinaryService) : ILessonService
{
    public async Task<LessonDto> CreateAsync(CreateLessonDto dto)
    {
        if (!userService.IsAuthenticated)
        {
            throw new Exception("Unauthorized");
        }

        if (userService.Role != UserRole.Admin && userService.Role != UserRole.Instructor)
        {
            throw new Exception("Only instructors and admins can create lessons.");
        }

        var module = await context.Modules.Include(x => x.Course).FirstOrDefaultAsync(x => x.Id == dto.ModuleId);

        if (module is null)
        {
            throw new Exception("Module not found.");
        }

        if (userService.Role != UserRole.Admin && module.Course.InstructorId != userService.UserId)
        {
            throw new Exception("Forbidden");
        }

        if (await context.Lessons.AnyAsync(x =>x.ModuleId == dto.ModuleId && x.Title.ToLower() == dto.Title.Trim().ToLower()))
        {
            throw new Exception("Lesson title already exists in this module.");
        }

        if (await context.Lessons.AnyAsync(x =>x.ModuleId == dto.ModuleId && x.Order == dto.Order))
        {
            throw new Exception("Lesson order already exists in this module.");
        }

        var uploadResult = await cloudinaryService.VideoUploadAsync(dto.File);

        Lesson lesson = new()
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            VideoUrl = uploadResult.SecureUrl.AbsoluteUri,
            PublicId = uploadResult.PublicId,
            Duration = uploadResult.Duration,
            Order = dto.Order,
            ModuleId = dto.ModuleId,
            CreatedAt = DateTime.UtcNow
        };

        context.Lessons.Add(lesson);
        await context.SaveChangesAsync();

        lesson = await context.Lessons.Include(x => x.Module).FirstAsync(x => x.Id == lesson.Id);
        return lesson.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        if (!userService.IsAuthenticated)
        {
            throw new Exception("Unauthorized");
        }

        if (userService.Role != UserRole.Admin && userService.Role != UserRole.Instructor)
        {
            throw new Exception("Only instructors and admins can delete lessons.");
        }

        var lesson = await context.Lessons.Include(x => x.Module).ThenInclude(x => x.Course).FirstOrDefaultAsync(x => x.Id == id);

        if (lesson is null)
        {
            throw new Exception("Lesson not found.");
        }

        if (userService.Role != UserRole.Admin && lesson.Module.Course.InstructorId != userService.UserId)
        {
            throw new Exception("Forbidden");
        }

        await cloudinaryService.DeleteVideoAsync(lesson.PublicId);

        context.Lessons.Remove(lesson);

        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<LessonDto>> GetByModuleAsync(int moduleId)
    {
        if (!await context.Modules.AnyAsync(x => x.Id == moduleId))
        {
            throw new Exception("Module not found.");
        }
        return await context.Lessons.Where(x=>x.ModuleId==moduleId).OrderBy(x=>x.Order).Include(x=>x.Module).Select(x=>x.ToDto()).ToListAsync();
    }

    public async Task<LessonDto> GetByIdAsync(int id)
    {
        var lesson=await context.Lessons.Include(x=>x.Module).FirstOrDefaultAsync(x=>x.Id==id);
        if(lesson is null) throw new Exception("Lesson not Found");
        return lesson.ToDto();
    }

    public async Task<LessonDto> UpdateAsync(int id, UpdateLessonDto dto)
    {
        if (!userService.IsAuthenticated)
        {
            throw new Exception("Unauthorized");
        }

        if (userService.Role != UserRole.Admin && userService.Role != UserRole.Instructor)
        {
            throw new Exception("Only instructors and admins can update lessons.");
        }

        var lesson = await context.Lessons.Include(x => x.Module).ThenInclude(x => x.Course).FirstOrDefaultAsync(x => x.Id == id);

        if (lesson is null)
        {
            throw new Exception("Lesson not found.");
        }

        if (userService.Role != UserRole.Admin && lesson.Module.Course.InstructorId != userService.UserId)
        {
            throw new Exception("Forbidden");
        }

        if (!await context.Modules.AnyAsync(x => x.Id == dto.ModuleId))
        {
            throw new Exception("Module not found.");
        }

        if (await context.Lessons.AnyAsync(x => x.ModuleId == dto.ModuleId && x.Title.ToLower() == dto.Title.Trim().ToLower() 
                && x.Id != id))
        {
            throw new Exception("Lesson title already exists in this module.");
        }

        if (await context.Lessons.AnyAsync(x =>x.ModuleId == dto.ModuleId && x.Order == dto.Order && x.Id != id))
        {
            throw new Exception("Lesson order already exists in this module.");
        }

        lesson.Title = dto.Title.Trim();
        lesson.Description = dto.Description?.Trim() ?? string.Empty;
        lesson.Order = dto.Order;
        lesson.ModuleId = dto.ModuleId;

        if (dto.File is not null && dto.File.Length > 0)
        {
            var uploadResult = await cloudinaryService.VideoUploadAsync(dto.File);

            await cloudinaryService.DeleteVideoAsync(lesson.PublicId);

            lesson.VideoUrl = uploadResult.SecureUrl.AbsoluteUri;
            lesson.PublicId = uploadResult.PublicId;
            lesson.Duration = uploadResult.Duration;
        }

        await context.SaveChangesAsync();

        lesson = await context.Lessons.Include(x => x.Module).FirstAsync(x => x.Id == lesson.Id);

        return lesson.ToDto();
    }
}