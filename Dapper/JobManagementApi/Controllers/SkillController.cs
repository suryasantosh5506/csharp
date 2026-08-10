using JobManagementApi.Dtos.Skills;
using JobManagementApi.Interfaces;
using JobManagementApi.RequestHelpers.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementApi.Controllers;

public class SkillController(ISkillService skillService) : BaseApiController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetAllSkills([FromQuery]PaginationParams paginationParams)
    {
        var skills=await skillService.GetAllSkillsAsync(paginationParams);

        return Ok(skills);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<SkillDto>> GetSkill(int id)
    {
        var skill=await skillService.GetSkillAsync(id);

        return Ok(skill);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SkillDto>> CreateSkill(CreateSkillDto dto)
    {
        var skill=await skillService.CreateSkillAsync(dto);

        return Ok(skill);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateSkill(int id,UpdateSkillDto dto)
    {
        await skillService.UpdateSkillAsync(id,dto);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        await skillService.DeleteSkillAsync(id);

        return NoContent();
    }
}