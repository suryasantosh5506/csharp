using JobManagementApi.Dtos.Skills;
using JobManagementApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobSkillController(IJobSkillService jobSkillService) : BaseApiController
{
    [HttpPost("{jobId:int}/skills/{skillId:int}")]
    [Authorize(Roles = "Recruiter,Admin")]
    public async Task<IActionResult> AddSkillToJob(int jobId,int skillId)
    {
        await jobSkillService.AddSkillToJob(jobId,skillId);

        return NoContent();
    }

    [HttpGet("{jobId:int}/skills")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetJobSkills(int jobId)
    {
        var skills=await jobSkillService.GetJobSkills(jobId);

        return Ok(skills);
    }

    [HttpDelete("{jobId:int}/skills/{skillId:int}")]
    [Authorize(Roles = "Recruiter,Admin")]
    public async Task<IActionResult> RemoveSkillFromJob(int jobId,int skillId)
    {
        await jobSkillService.RemoveSkillFromJob(jobId,skillId);

        return NoContent();
    }
}