using System.Security.Claims;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Patient;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles = "Patient")]
public class PatientProfileController(HospitalContext context,IPatientProfileService patientProfileService) : BaseApiController
{
    private async Task<Patient?> GetCurrentPatientAsync()
    {
        var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await context.Patients.FirstOrDefaultAsync(x=>x.UserId==userId);
    }

    [HttpGet]
    public async Task<ActionResult<PatientDetailsDto>> GetProfileAsync()
    {
        var patient=await GetCurrentPatientAsync();

        if(patient is null) return NotFound("Patient profile not found.");

        var profile=await patientProfileService.GetProfileAsync(patient.UserId);

        return Ok(profile);
    }

    [HttpPost]
    public async Task<ActionResult<PatientDetailsDto>> CreateProfileAsync(CreatePatientDto dto)
    {
        var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if(await context.Patients.AnyAsync(x=>x.UserId==userId))
            return BadRequest("Patient profile already exists.");

        var patient=await patientProfileService.CreateProfileAsync(userId,dto);

        return CreatedAtAction(nameof(GetProfileAsync),patient);
    }

    [HttpPut]
    public async Task<ActionResult<PatientDetailsDto>> UpdateProfileAsync(UpdatePatientDto dto)
    {
        var patient=await GetCurrentPatientAsync();

        if(patient is null)
            return NotFound("Patient profile not found.");

        var updatedPatient=await patientProfileService.UpdateProfileAsync(patient.UserId,dto);

        return Ok(updatedPatient);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProfileAsync()
    {
        var patient=await GetCurrentPatientAsync();

        if(patient is null)
            return NotFound();

        var deleted=await patientProfileService.DeleteProfileAsync(patient.UserId);

        if(!deleted)
            return NotFound();

        return NoContent();
    }
}