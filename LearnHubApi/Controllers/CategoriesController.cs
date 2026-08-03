using LearnHubApi.Dtos.Category;
using LearnHubApi.Extensions;
using LearnHubApi.Interfaces;
using LearnHubApi.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHubApi.Controllers;

public class CategoriesController(ICategoryService categoryService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories([FromQuery]PaginationParams paginationParams)
    {
        var categories = await categoryService.GetAllAsync(paginationParams);
        Response.AddPaginationHeader(categories.paginationMetaData);
        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await categoryService.GetByIdAsync(id);
        return Ok(category);
    }

    [Authorize(Roles ="Admin")]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
    {
        var category = await categoryService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.Id },
            category
        );
    }

    [Authorize(Roles ="Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, UpdateCategoryDto dto)
    {
        var category = await categoryService.UpdateAsync(id, dto);
        return Ok(category);
    }

    [Authorize(Roles ="Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await categoryService.DeleteAsync(id);
        return NoContent();
    }
}