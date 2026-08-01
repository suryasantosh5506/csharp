using System.Security.Claims;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.DoctorApplication;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.enums;
using HospitalManagementAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles =nameof(UserRole.Patient))]
public class DoctorApplicationController(HospitalContext context,IDoctorApplicationService doctorApplicationService): BaseApiController
{
    private async Task<Patient?> GetCurrentPatientAsync()
    {
        var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await context.Patients.FirstOrDefaultAsync(x=>x.UserId==userId);
    }

    [HttpGet]
    public async Task<ActionResult<DoctorApplicationDetailsDto>> GetMyApplication()
    {
        var patient=await GetCurrentPatientAsync();

        if(patient is null) return Unauthorized();

        var application=await doctorApplicationService.GetMyApplicationAsync(patient.UserId);

        if(application is null)
            return NotFound("You have not applied for doctor");

        return Ok(application);
    }

    [HttpPost]
    public async Task<ActionResult<DoctorApplicationDetailsDto>> ApplyDoctorAsync(CreateDoctorApplicationDto applicationDto)
    {
        var patient=await GetCurrentPatientAsync();

        if(patient is null)
            return Unauthorized();

        var isDoctor=await context.Doctors.AnyAsync(x=>x.UserId==patient.UserId);

        if(isDoctor)
            return BadRequest("You are already a doctor.");

        if(await context.DoctorApplications.AnyAsync(x=>x.UserId==patient.UserId && x.Status==DoctorApplicationStatus.Pending))
            return BadRequest("Application already pending.");

        var application=await doctorApplicationService.ApplyDoctorAsync(patient.UserId,applicationDto);

        return CreatedAtAction(nameof(GetMyApplication),application);
    }
}