using System.Security.Claims;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Patient;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles = "Patient")]
public class PatientProfileController(HospitalContext context) : BaseApiController
{
    private async Task<Patient?> GetCurrentPatientAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await context.Patients.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    [HttpGet]
    public async Task<ActionResult<PatientDetailsDto>> GetProfileAsync()
    {
        var patient = await GetCurrentPatientAsync();
        if (patient is null) return NotFound("Patient profile not found.");
        return Ok(patient.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<PatientDetailsDto>> CreateProfileAsync(CreatePatientDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var exists = await context.Patients.AnyAsync(x => x.UserId == userId);

        if (exists) return BadRequest("Patient profile already exists.");

        var patient = new Patient
        {
            UserId = userId,

            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,

            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            BloodGroup = dto.BloodGroup,
            Height = dto.Height,
            Weight = dto.Weight,
            Address = dto.Address,
            EmergencyContactName = dto.EmergencyContactName,
            EmergencyContactPhone = dto.EmergencyContactPhone
        };

        context.Patients.Add(patient);

        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProfileAsync), patient.ToDto());
    }

    [HttpPut]
    public async Task<ActionResult<PatientDetailsDto>> UpdateProfileAsync(UpdatePatientDto dto)
    {
        var patient = await GetCurrentPatientAsync();

        if (patient is null) return NotFound("Patient profile not found.");

        patient.FirstName = dto.FirstName;
        patient.LastName = dto.LastName;
        patient.Email = dto.Email;
        patient.PhoneNumber = dto.PhoneNumber;

        patient.DateOfBirth = dto.DateOfBirth;
        patient.Gender = dto.Gender;
        patient.BloodGroup = dto.BloodGroup;
        patient.Height = dto.Height;
        patient.Weight = dto.Weight;
        patient.Address = dto.Address;
        patient.EmergencyContactName = dto.EmergencyContactName;
        patient.EmergencyContactPhone = dto.EmergencyContactPhone;

        await context.SaveChangesAsync();

        return Ok(patient.ToDto());
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProfileAsync()
    {
        var patient = await GetCurrentPatientAsync();

        if (patient is null) return NotFound();

        context.Patients.Remove(patient);

        await context.SaveChangesAsync();

        return NoContent();
    }
}