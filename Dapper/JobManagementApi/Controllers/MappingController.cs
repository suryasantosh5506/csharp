using JobManagementApi.Dtos.Company;
using JobManagementApi.Dtos.JobApplication;
using JobManagementApi.Dtos.Jobs;
using JobManagementApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MappingController(IMappingService mappingService) : BaseApiController
{
    [HttpGet("job/{jobId:int}")]
    public async Task<ActionResult<JobDetailsDto>> GetJobDetails(int jobId)
    {
        var job=await mappingService.GetJobDetails(jobId);

        return Ok(job);
    }

    [HttpGet("application/{applicationId:int}")]
    public async Task<ActionResult<JobApplicationDetailsDto>> GetApplicationDetails(int applicationId)
    {
        var application=await mappingService.GetApplicationDetails(applicationId);

        return Ok(application);
    }

    [HttpGet("company/{companyId:int}")]
    public async Task<ActionResult<CompanyDetailsDto>> GetCompanyDetails(int companyId)
    {
        var company=await mappingService.GetCompanyDetails(companyId);

        return Ok(company);
    }

    [HttpGet("job/{jobId:int}/skills")]
    public async Task<ActionResult<JobWithSkillsDetailsDto>> GetJobWithSkillsDetails(int jobId)
    {
        var job=await mappingService.GetJobWithSkillsDetails(jobId);

        return Ok(job);
    }
}