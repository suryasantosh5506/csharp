using System.Security.Claims;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.DoctorApplication;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles = "Patient")]
public class DoctorApplicationController(HospitalContext context): BaseApiController
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
        var application=await context.DoctorApplications.Include(x=>x.User).FirstOrDefaultAsync(x=>x.UserId==patient.UserId);
        if(application is null) return NotFound("You have not applied for doctor");
        return Ok(application.ToDto());
    }


    [HttpPost]
    public async Task<ActionResult<DoctorApplicationDetailsDto>> ApplyDoctorAsync(CreateDoctorApplicationDto applicationDto)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized();
        var isDoctor = await context.Doctors.AnyAsync(x => x.UserId == patient.UserId);
        if(isDoctor) return BadRequest("You are already a doctor.");   
        if(await context.DoctorApplications.AnyAsync(x=>x.UserId==patient.UserId && x.Status=="pending")) return BadRequest("Application already pending.");
        var application = new DoctorApplication
        {
            UserId = patient.UserId,
            Specialization = applicationDto.Specialization,
            Qualification = applicationDto.Qualification,
            YearsOfExperience = applicationDto.YearsOfExperience,
            HospitalName = applicationDto.HospitalName,
            Bio = applicationDto.Bio,
            LicenseNumber = applicationDto.LicenseNumber,
            Status = "Pending",
            AppliedAt = DateTime.UtcNow
        };
        context.DoctorApplications.Add(application);
        await context.SaveChangesAsync();
        return CreatedAtRoute(nameof(GetMyApplication),application.ToDto());
    }
}