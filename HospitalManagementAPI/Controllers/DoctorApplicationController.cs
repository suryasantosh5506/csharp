using System.Security.Claims;
using HospitalManagementAPI.Data;
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

    [HttpPost]
    public async Task<ActionResult<DoctorApplicationDetailsDto>> ApplyDoctorAsync(CreateDoctorApplicationDto applicationDto)
    {
        var patient=await GetCurrentPatientAsync();
        if(patient is null) return Unauthorized();
        if(await context.DoctorApplications.AnyAsync(x=>x.UserId==patient.UserId)) return BadRequest("Application already pending.");
        var isDoctor = await context.Doctors.AnyAsync(x => x.UserId == patient.UserId);
        if(isDoctor) return BadRequest("You are already a doctor.");   
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
        return CreatedAtAction(nameof(GetMyApplication),new { id = application.Id },application.ToDto());
    }
}