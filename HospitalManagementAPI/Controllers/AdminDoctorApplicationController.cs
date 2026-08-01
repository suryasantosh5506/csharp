using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.DoctorApplication;
using HospitalManagementAPI.enums;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles =nameof(UserRole.Admin))]
public class AdminDoctorApplicationController(
    HospitalContext context,
    IAdminDoctorApplicationService adminDoctorApplicationService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<DoctorApplicationDetailsDto>>> GetAllApplicationsAsync([FromQuery]PaginationParams paginationParams)
    {
        var applications=await adminDoctorApplicationService.GetAllApplicationsAsync(paginationParams);

        return Ok(applications);
    }

    [HttpGet("pending")]
    public async Task<ActionResult<PagedList<DoctorApplicationDetailsDto>>> GetAllPendingApplicationsAsync([FromQuery]PaginationParams paginationParams)
    {
        var applications=await adminDoctorApplicationService.GetAllPendingApplicationsAsync(paginationParams);

        return Ok(applications);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DoctorApplicationDetailsDto>> GetApplicationByIdAsync(int id)
    {
        var application=await adminDoctorApplicationService.GetApplicationByIdAsync(id);

        if(application is null)
            return NotFound();

        return Ok(application);
    }

    [HttpPatch("{id:int}/reject")]
    public async Task<ActionResult<DoctorApplicationDetailsDto>> RejectApplicationAsync(int id)
    {
        var application=await adminDoctorApplicationService.GetApplicationByIdAsync(id);

        if(application is null)
            return NotFound();

        if(application.Status!=DoctorApplicationStatus.Pending)
            return BadRequest("Application has already been processed.");

        var updatedApplication=await adminDoctorApplicationService.RejectApplicationAsync(id);

        return Ok(updatedApplication);
    }

    [HttpPatch("{id:int}/approve")]
    public async Task<ActionResult<DoctorApplicationDetailsDto>> ApproveApplicationAsync(int id)
    {
        var application=await adminDoctorApplicationService.GetApplicationByIdAsync(id);

        if(application is null)
            return NotFound();

        if(application.Status!=DoctorApplicationStatus.Pending)
            return BadRequest("Application has already been processed.");

        var updatedApplication=await adminDoctorApplicationService.ApproveApplicationAsync(id);

        return Ok(updatedApplication);
    }
}