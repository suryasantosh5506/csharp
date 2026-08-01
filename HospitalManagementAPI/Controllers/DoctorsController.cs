using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Doctor;
using HospitalManagementAPI.enums;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles =nameof(UserRole.Admin))]
public class DoctorsController(HospitalContext context,IDoctorService doctorService):BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<DoctorDetailsDto>>> GetAllDoctorsAsync([FromQuery]PaginationParams paginationParams)
    {
        var doctors=await doctorService.GetAllDoctorsAsync(paginationParams);
        return Ok(doctors);
    }

    [HttpGet("{id:int}",Name ="GetDoctorById")]
    public async Task<ActionResult<DoctorDetailsDto>> GetDoctorByIdAsync(int id)
    {
        var doctor=await doctorService.GetDoctorByIdAsync(id);
        if(doctor is null) return NotFound();
        return Ok(doctor);
    }

    [HttpPost]
    public async Task<ActionResult<DoctorDetailsDto>> CreateDoctorAsync(CreateDoctorDto newDoctorDto)
    {
        if(!await context.Departments.AnyAsync(x=>x.Id==newDoctorDto.DepartmentId))
        {
            return BadRequest();
        }

        if(await context.Doctors.AnyAsync(x=>x.Email==newDoctorDto.Email || x.PhoneNumber==newDoctorDto.PhoneNumber ||
                x.LicenseNumber==newDoctorDto.LicenseNumber))
        {
            return Conflict();
        }

        var doctor=await doctorService.CreateDoctorAsync(newDoctorDto);

        return CreatedAtRoute("GetDoctorById",new {id=doctor.Id},doctor);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DoctorDetailsDto>> UpdateDoctorAsync(int id,UpdateDoctorDto updateDoctorDto)
    {
        if(!await context.Departments.AnyAsync(x=>x.Id==updateDoctorDto.DepartmentId))
        {
            return BadRequest();
        }

        if(await context.Doctors.AnyAsync(x=>(x.Email==updateDoctorDto.Email ||
                                                x.PhoneNumber==updateDoctorDto.PhoneNumber ||
                                                x.LicenseNumber==updateDoctorDto.LicenseNumber) &&
                                                x.Id!=id))
        {
            return Conflict();
        }

        var doctor=await doctorService.UpdateDoctorAsync(id,updateDoctorDto);

        if(doctor is null) return NotFound();

        return Ok(doctor);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteDoctorAsync(int id)
    {
        var deleted=await doctorService.DeleteDoctorAsync(id);

        if(!deleted) return NotFound();

        return NoContent();
    }
}