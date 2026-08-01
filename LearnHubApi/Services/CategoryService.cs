using LearnHubApi.Data;
using LearnHubApi.Dtos.Category;
using LearnHubApi.Entities;
using LearnHubApi.Extensions;
using LearnHubApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnHubApi.Services;

public class CategoryService(AppDbContext context) : ICategoryService
{
    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        if (await context.Categories.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower()))
        {
            throw new Exception("Category already exists");
        }

        Category category = new()
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        return category.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
        {
            throw new Exception("Category not found");
        }

        if (await context.Courses.AnyAsync(x => x.CategoryId == id))
        {
            throw new Exception("Cannot delete category with existing courses.");
        }

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        return await context.Categories
            .Select(x => x.ToDto())
            .ToListAsync();
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
        {
            throw new Exception("Category not found");
        }

        return category.ToDto();
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        if (await context.Categories.AnyAsync(x =>
            x.Name.ToLower() == dto.Name.ToLower() &&
            x.Id != id))
        {
            throw new Exception("Category already exists");
        }

        var category = await context.Categories
            .FirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
        {
            throw new Exception("Category not found");
        }

        category.Name = dto.Name.Trim();
        category.Description = dto.Description?.Trim() ?? string.Empty;

        await context.SaveChangesAsync();

        return category.ToDto();
    }
}