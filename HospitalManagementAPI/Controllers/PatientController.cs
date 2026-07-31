using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Patient;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles ="Admin")]
public class PatientController(HospitalContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<PatientDetailsDto>>> GetAllPatientsAsync([FromQuery]PaginationParams paginationParams)
    {
        var query=context.Patients.Select(x=>x.ToDto());
        var patients=await PagedList<PatientDetailsDto>.ToPagedList(query,paginationParams.pageNumber,paginationParams.pageSize);
        return Ok(patients);
    }

    [HttpGet("{id:int}",Name ="GetPatientById")]
    public async Task<ActionResult<PatientDetailsDto>> GetPatientByIdAsync(int id)
    {
        var patient = await context.Patients.FindAsync(id);
        if(patient is null) return NotFound();
        return Ok(patient.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<PatientDetailsDto>> CreatePatientAsync(CreatePatientDto newPatient)
    {
        if(await context.Patients.AnyAsync(x=>x.Email==newPatient.Email || x.PhoneNumber == newPatient.PhoneNumber))
        {
            return Conflict();
        }

        var patient=new Patient()
        {
            FirstName=newPatient.FirstName.Trim(),
            LastName=newPatient.LastName.Trim(),
            Email=newPatient.Email.Trim(),
            PhoneNumber=newPatient.PhoneNumber.Trim(),
            DateOfBirth=newPatient.DateOfBirth,
            Gender=newPatient.Gender.Trim(),
            BloodGroup=newPatient.BloodGroup.Trim(),
            Height=newPatient.Height,
            Weight=newPatient.Weight,
            Address=newPatient.Address.Trim(),
            EmergencyContactName=newPatient.EmergencyContactName.Trim(),
            EmergencyContactPhone=newPatient.EmergencyContactPhone.Trim(),
        };
        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        return CreatedAtRoute("GetPatientById",new {id=patient.Id},patient.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PatientDetailsDto>> UpdatePatientAsync(int id,UpdatePatientDto updatePatientDto)
    {
        var patient=await context.Patients.FindAsync(id);

        if(patient is null) return NotFound();

        if(await context.Patients.AnyAsync(x=>(x.Email==updatePatientDto.Email || x.PhoneNumber==updatePatientDto.PhoneNumber) && x.Id != id))
        {
            return Conflict();
        }

        patient.FirstName=updatePatientDto.FirstName.Trim();
        patient.LastName=updatePatientDto.LastName.Trim();
        patient.Email=updatePatientDto.Email.Trim();
        patient.PhoneNumber=updatePatientDto.PhoneNumber.Trim();
        patient.DateOfBirth=updatePatientDto.DateOfBirth;
        patient.Gender=updatePatientDto.Gender.Trim();
        patient.BloodGroup=updatePatientDto.BloodGroup.Trim();
        patient.Height=updatePatientDto.Height;
        patient.Weight=updatePatientDto.Weight;
        patient.Address=updatePatientDto.Address.Trim();
        patient.EmergencyContactName=updatePatientDto.EmergencyContactName.Trim();
        patient.EmergencyContactPhone=updatePatientDto.EmergencyContactPhone.Trim();

        await context.SaveChangesAsync();
        return Ok(patient.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePatientAsync(int id)
    {
        var patient=await context.Patients.FindAsync(id);
        if(patient is null) return NotFound();
        context.Patients.Remove(patient);
        await context.SaveChangesAsync();
        return NoContent();
    }
}