using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Patient;
using HospitalManagementAPI.enums;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles =nameof(UserRole.Admin))]
public class PatientController(HospitalContext context,IPatientService patientService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<PatientDetailsDto>>> GetAllPatientsAsync([FromQuery]DoctorParams doctorParams)
    {
        var patients=await patientService.GetAllPatientsAsync(doctorParams);
        return Ok(patients);
    }

    [HttpGet("{id:int}",Name ="GetPatientById")]
    public async Task<ActionResult<PatientDetailsDto>> GetPatientByIdAsync(int id)
    {
        var patient=await patientService.GetPatientByIdAsync(id);
        if(patient is null) return NotFound();
        return Ok(patient);
    }

    [HttpPost]
    public async Task<ActionResult<PatientDetailsDto>> CreatePatientAsync(CreatePatientDto newPatient)
    {
        if(await context.Patients.AnyAsync(x=>x.Email==newPatient.Email || x.PhoneNumber==newPatient.PhoneNumber))
        {
            return Conflict();
        }
        var patient=await patientService.CreatePatientAsync(newPatient);
        return CreatedAtRoute("GetPatientById",new {id=patient.Id},patient);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PatientDetailsDto>> UpdatePatientAsync(int id,UpdatePatientDto updatePatientDto)
    {
        if(await context.Patients.AnyAsync(x=>
            (x.Email==updatePatientDto.Email ||
             x.PhoneNumber==updatePatientDto.PhoneNumber) &&
             x.Id!=id))
        {
            return Conflict();
        }
        var patient=await patientService.UpdatePatientAsync(id,updatePatientDto);
        if(patient is null) return NotFound();
        return Ok(patient);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePatientAsync(int id)
    {
        var deleted=await patientService.DeletePatientAsync(id);
        if(!deleted) return NotFound();
        return NoContent();
    }
}