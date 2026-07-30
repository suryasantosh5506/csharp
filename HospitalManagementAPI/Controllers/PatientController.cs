using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Patient;
using HospitalManagementAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

public class PatientController(HospitalContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<PatientDetailsDto>>> GetAllPatientsAsync()
    {
        var patients=await context.Patients.Select(x=>x.ToDto()).ToListAsync();
        return Ok(patients);
    }

    [HttpGet("{id:int}",Name ="GetPatientById")]
    public async Task<ActionResult<PatientDetailsDto>> GetPatientByIdAsync(int id)
    {
        var patient = await context.Patients.FindAsync(id);
        if(patient is null) return NotFound();
        return Ok(patient.ToDto());
    }
}