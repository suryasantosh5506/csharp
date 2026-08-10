using JobManagementApi.Dtos.RecruiterApplication;
using JobManagementApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecruiterApplicationController(IRecruiterApplicationService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateApplication(CreateRecruiterApplicationDto dto)
    {
        var application=await service.CreateApplication(dto);
        return Ok(application);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyApplications()
    {
        var applications=await service.GetMyApplications();
        return Ok(applications);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplicationById(int id)
    {
        var application=await service.GetApplicationById(id);
        return Ok(application);
    }

    [HttpGet]
    public async Task<IActionResult> GetApplications()
    {
        var applications=await service.GetApplications();
        return Ok(applications);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateApplication(int id,UpdateRecruiterApplicationDto dto)
    {
        await service.UpdateApplication(id,dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        await service.DeleteApplication(id);
        return NoContent();
    }
}