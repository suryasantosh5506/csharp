using JobManagementApi.Dtos.RecruiterApplication;
using JobManagementApi.Interfaces;
using JobManagementApi.RequestHelpers.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecruiterApplicationController(IRecruiterApplicationService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RecruiterApplicationDto>> CreateApplication(CreateRecruiterApplicationDto dto)
    {
        var application=await service.CreateApplication(dto);
        return Ok(application);
    }

    [HttpGet("my")]
    public async Task<ActionResult<PagedList<RecruiterApplicationDto>>> GetMyApplications([FromQuery] PaginationParams paginationParams)
    {
        var applications=await service.GetMyApplications(paginationParams);
        return Ok(applications);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecruiterApplicationDto>> GetApplicationById(int id)
    {
        var application=await service.GetApplicationById(id);
        return Ok(application);
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<RecruiterApplicationDto>>> GetApplications([FromQuery] PaginationParams paginationParams)
    {
        var applications=await service.GetApplications(paginationParams);
        return Ok(applications);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateApplication(int id,UpdateRecruiterApplicationDto dto)
    {
        await service.UpdateApplication(id,dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteApplication(int id)
    {
        await service.DeleteApplication(id);
        return NoContent();
    }
}