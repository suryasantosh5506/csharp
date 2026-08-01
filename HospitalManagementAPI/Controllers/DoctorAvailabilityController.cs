using System.Security.Claims;
using HospitalManagementAPI.Controllers;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.DoctorAvailability;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.enums;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles = nameof(UserRole.Doctor))]
public class DoctorAvailabilityController(HospitalContext context,IDoctorAvailabilityService doctorAvailabilityService): BaseApiController
{
    private async Task<Doctor?> GetCurrentDoctorAsync()
    {
        var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await context.Doctors.FirstOrDefaultAsync(x=>x.UserId==userId);
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<DoctorAvailabilityDetailsDto>>> GetAllAvailabilitesAsync([FromQuery]PaginationParams paginationParams)
    {
        var doctor=await GetCurrentDoctorAsync();
        if(doctor is null) return Unauthorized();
        var availabilities=await doctorAvailabilityService.GetDoctorAvailabilityAsync(doctor.Id,paginationParams);
        return Ok(availabilities);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PagedList<DoctorAvailabilityDetailsDto>>> GetAvailabilityByIdAsync(int id)
    {
        var doctor=await GetCurrentDoctorAsync();
        if(doctor is null) return Unauthorized();
        var availability=await doctorAvailabilityService.GetAvailabilityByIdAsync(id,doctor.Id);
        if(availability is null) return NotFound();
        return Ok(availability);
    }

    [HttpPost]
    public async Task<ActionResult<DoctorAvailabilityDetailsDto>> CreateDoctorAvailabilityAsync(CreateDoctorAvailabilityDto dto)
    {
        var doctor=await GetCurrentDoctorAsync();
        if(doctor is null) return Unauthorized();
        var availability=await doctorAvailabilityService.CreateAvailabilityAsync(doctor.Id,dto);
        if(availability is null) return Conflict();
        return Ok(availability);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DoctorAvailabilityDetailsDto>> UpdateDoctorAvailabilityAsync(int id,UpdateDoctorAvailabilityDto dto)
    {
        var doctor=await GetCurrentDoctorAsync();
        if(doctor is null) return Unauthorized();
        var availability=await doctorAvailabilityService.UpdateAvailabilityAsync(id,doctor.Id,dto);
        if(availability is null) return BadRequest();
        return Ok(availability);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteDoctorAvailabilityAsync(int id)
    {
        var doctor=await GetCurrentDoctorAsync();
        if(doctor is null) return Unauthorized();
        var result=await doctorAvailabilityService.DeleteAvailabilityAsync(id,doctor.Id);
        if(!result) return BadRequest();
        return NoContent();
    }
}
