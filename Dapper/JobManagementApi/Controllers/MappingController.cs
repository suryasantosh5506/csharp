using JobManagementApi.Dtos.Jobs;
using JobManagementApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MappingController(IMappingService mappingService) : BaseApiController
{
    [HttpGet("{jobId:int}")]
    public async Task<ActionResult<JobDetailsDto>> GetJobDetails(int jobId)
    {
        var job=await mappingService.GetJobDetails(jobId);

        return Ok(job);
    }
}