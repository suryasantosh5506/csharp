using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Doctor;
using HospitalManagementAPI.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

public class DoctorsController(HospitalContext context):BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<DoctorDetailsDto>>> GetAllDoctorsAsync()
    {
        var doctors=await context.Doctors.Include(x=>x.Department).Select(x=>x.ToDto()).ToListAsync();
        return Ok(doctors);
    }

    [HttpGet("{id:int}",Name ="GetDoctorById")]
    public async Task<ActionResult<DoctorDetailsDto>> GetDoctorByIdAsync(int id)
    {
        var doctor=await context.Doctors.Include(x=>x.Department).FirstOrDefaultAsync(x=>x.Id==id);
        if(doctor is null) return NotFound();
        return Ok(doctor.ToDto());
    }
}