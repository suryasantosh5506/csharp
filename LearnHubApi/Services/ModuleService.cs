using LearnHubApi.Data;
using LearnHubApi.Dtos.Modules;
using LearnHubApi.Enums;
using LearnHubApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using LearnHubApi.Entities;
using LearnHubApi.Extensions;
using LearnHubApi.Exceptions;

namespace LearnHubApi.Services;

public class ModuleService(ICurrentUserService userService,AppDbContext context) : IModuleService
{
    public async Task<ModuleDto> CreateAsync(CreateModuleDto dto)
{
    if (!userService.IsAuthenticated)
    {
        throw new UnauthorizedException("Unauthorized");
    }

    if (userService.Role != UserRole.Instructor && userService.Role != UserRole.Admin)
    {
        throw new ForbiddenException("Only instructors and admins can create modules.");
    }

    var course = await context.Courses.FirstOrDefaultAsync(x => x.Id == dto.CourseId);

    if (course is null)
    {
        throw new NotFoundException("Course not found.");
    }

    if (userService.Role != UserRole.Admin && userService.UserId != course.InstructorId)
    {
        throw new ForbiddenException("You are not allowed to add modules to this course.");
    }

    if (await context.Modules.AnyAsync(x =>x.CourseId == dto.CourseId && x.Title.ToLower() == dto.Title.Trim().ToLower()))
    {
        throw new ConflictException("A module with the same title already exists in this course.");
    }

    if (await context.Modules.AnyAsync(x =>x.CourseId == dto.CourseId && x.Order == dto.Order))
    {
        throw new ConflictException("Module order already exists in this course.");
    }

    Module module = new()
    {
        Title = dto.Title.Trim(),
        Description = dto.Description?.Trim() ?? string.Empty,
        Order = dto.Order,
        CourseId = dto.CourseId,
        CreatedAt = DateTime.UtcNow
    };

    context.Modules.Add(module);
    await context.SaveChangesAsync();

    module = await context.Modules.Include(x => x.Course).Include(x => x.Lessons).FirstAsync(x => x.Id == module.Id);

    return module.ToDto();
}

    public async Task DeleteAsync(int id)
    {
        if(!userService.IsAuthenticated) throw new UnauthorizedException("Unauthorized.");
        var module=await context.Modules.Include(x=>x.Course).Include(x=>x.Lessons).FirstOrDefaultAsync(x=>x.Id==id);
        if(module is null)
        {
            throw new NotFoundException("Module not found.");
        }
        if(userService.Role!=UserRole.Admin && userService.Role!=UserRole.Instructor) 
            throw new ForbiddenException("Not Allowed");
        if(module.Course.InstructorId!=userService.UserId && userService.Role!=UserRole.Admin) 
            throw new ForbiddenException("Forbidden");
        if (module.Lessons.Any()) throw new ConflictException("Cannot delete module with existing lessons.");
        context.Modules.Remove(module);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ModuleDto>> GetAllAsync()
    {
        return await context.Modules.Include(x=>x.Course).Include(x=>x.Lessons).Select(x=>x.ToDto()).ToListAsync();
    }

    public async Task<ModuleDto> GetByIdAsync(int id)
    {
         var module=await context.Modules.Include(x=>x.Course).Include(x=>x.Lessons).FirstOrDefaultAsync(x=>x.Id==id);
        if(module is null)
        {
            throw new NotFoundException("Module not found.");
        }
        return module.ToDto();
    }

    public async Task<ModuleDto> UpdateAsync(int id, UpdateModuleDto dto)
    {
        if (!userService.IsAuthenticated)
        {
            throw new UnauthorizedException("Unauthorized");
        }

        if (userService.Role != UserRole.Instructor && userService.Role != UserRole.Admin)
        {
            throw new ForbiddenException("Only instructors and admins can create modules.");
        }

        var course = await context.Courses.FirstOrDefaultAsync(x => x.Id == dto.CourseId);

        if (course is null)
        {
            throw new NotFoundException("Course not found.");
        }

        var module=await context.Modules.Include(x=>x.Course).Include(x=>x.Lessons).FirstOrDefaultAsync(x=>x.CourseId==dto.CourseId && x.Id==id);

        if (module is null)
        {
            throw new NotFoundException("Module not found.");
        }

        if (userService.Role != UserRole.Admin && userService.UserId != course.InstructorId)
        {
            throw new ForbiddenException("You are not allowed to add modules to this course.");
        }

        if (await context.Modules.AnyAsync(x =>x.CourseId == dto.CourseId && x.Title.ToLower() == dto.Title.Trim().ToLower() && 
                    x.Id != id))
        {
            throw new ConflictException("A module with the same title already exists in this course.");
        }

        if (await context.Modules.AnyAsync(x =>x.CourseId == dto.CourseId && x.Order == dto.Order && x.Id != id))
        {
            throw new ConflictException("Module order already exists in this course.");
        }

        module.Title=dto.Title.Trim();
        module.Description=dto.Description?.Trim()??string.Empty;    
        module.Order=dto.Order;
        module.CourseId=dto.CourseId;
        await context.SaveChangesAsync();

        module = await context.Modules.Include(x => x.Course).Include(x => x.Lessons).FirstAsync(x => x.Id == module.Id);

        return module.ToDto();
    }
}