using LearnHubApi.Dtos.Modules;
using LearnHubApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHubApi.Controllers;

public class ModuleController(IModuleService moduleService) : BaseApiController
{
    [HttpGet("course/{courseId:int}")]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModulesByCourseAsync(int courseId)
    {
        return Ok(await moduleService.GetByCourseAsync(courseId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ModuleDto>> GetModuleByIdAsync(int id)
    {
        return Ok(await moduleService.GetByIdAsync(id));
    }

    [Authorize(Roles = "Admin,Instructor")]
    [HttpPost]
    public async Task<ActionResult<ModuleDto>> CreateModuleAsync(CreateModuleDto dto)
    {
        var module = await moduleService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetModuleByIdAsync),
            new { id = module.Id },
            module);
    }

    [Authorize(Roles = "Admin,Instructor")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ModuleDto>> UpdateModuleAsync(
        int id,
        UpdateModuleDto dto)
    {
        return Ok(await moduleService.UpdateAsync(id, dto));
    }

    [Authorize(Roles = "Admin,Instructor")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteModuleAsync(int id)
    {
        await moduleService.DeleteAsync(id);
        return NoContent();
    }
}