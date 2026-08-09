using JobManagementApi.Dtos.JobApplication;
using JobManagementApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementApi.Controllers;

public class JobApplicationController(IJobApplicationService jobApplicationService) : BaseApiController
{
    [HttpPost("{jobId:int}")]
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<JobApplicationDto>> CreateApplication(
        int jobId,
        CreateJobApplicationDto dto)
    {
        var application = await jobApplicationService.CreateApplication(jobId, dto);

        return Ok(application);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<JobApplicationDto>> GetApplicationById(int id)
    {
        var application = await jobApplicationService.GetApplicationById(id);

        return Ok(application);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<IEnumerable<JobApplicationDto>>> GetMyApplications()
    {
        var applications = await jobApplicationService.GetMyApplications();

        return Ok(applications);
    }

    [HttpGet("job/{jobId:int}")]
    [Authorize(Roles = "Recruiter,Admin")]
    public async Task<ActionResult<IEnumerable<JobApplicationDto>>> GetJobApplications(int jobId)
    {
        var applications = await jobApplicationService.GetJobApplications(jobId);

        return Ok(applications);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Recruiter,Admin")]
    public async Task<IActionResult> UpdateApplicationStatus(
        int id,
        UpdateJobApplicationDto dto)
    {
        await jobApplicationService.UpdateApplicationStatus(id, dto);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Candidate,Admin")]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        await jobApplicationService.DeleteApplication(id);

        return NoContent();
    }
}