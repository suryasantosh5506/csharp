using JobManagementApi.Dtos.Jobs;
using JobManagementApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementApi.Controllers;

public class JobController(IJobService jobService) : BaseApiController
{
    [HttpPost]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<JobDto>> CreateJob(CreateJobDto dto)
    {
        var job = await jobService.CreateJob(dto);

        return Ok(job);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<JobDto>>> GetJobs()
    {
        var jobs = await jobService.GetJobs();

        return Ok(jobs);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<JobDto>> GetJobById(int id)
    {
        var job = await jobService.GetJobById(id);

        return Ok(job);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<IActionResult> UpdateJob(
        int id,
        UpdateJobDto dto)
    {
        await jobService.UpdateJob(id, dto);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<IActionResult> DeleteJob(int id)
    {
        await jobService.DeleteJob(id);

        return NoContent();
    }
}