using System.Runtime.InteropServices;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Doctor;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles = "Admin")]
public class DoctorsController(HospitalContext context):BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<DoctorDetailsDto>>> GetAllDoctorsAsync([FromQuery]PaginationParams paginationParams)
    {
        var query=context.Doctors.Include(x=>x.Department).Select(x=>x.ToDto());
        var doctors=await PagedList<DoctorDetailsDto>.ToPagedList(query,paginationParams.pageNumber,paginationParams.pageSize);
        return Ok(doctors);
    }

    [HttpGet("{id:int}",Name ="GetDoctorById")]
    public async Task<ActionResult<DoctorDetailsDto>> GetDoctorByIdAsync(int id)
    {
        var doctor=await context.Doctors.Include(x=>x.Department).FirstOrDefaultAsync(x=>x.Id==id);
        if(doctor is null) return NotFound();
        return Ok(doctor.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<DoctorDetailsDto>> CreateDoctorAsync(CreateDoctorDto newDoctorDto)
    {
        if(!await context.Departments.AnyAsync(x=>x.Id==newDoctorDto.DepartmentId))
        {
            return BadRequest();
        }

        if(await context.Doctors.AnyAsync(x=>x.Email==newDoctorDto.Email || x.PhoneNumber == newDoctorDto.PhoneNumber ||
                x.LicenseNumber == newDoctorDto.LicenseNumber))
        {
            return Conflict();
        }

        Doctor doctor = new()
        {
            FirstName=newDoctorDto.FirstName.Trim(),
            LastName=newDoctorDto.LastName.Trim(),
            Email=newDoctorDto.Email.Trim(),
            PhoneNumber=newDoctorDto.PhoneNumber.Trim(),
            Qualification=newDoctorDto.Qualification.Trim(),
            Specialization=newDoctorDto.Specialization.Trim(),
            ExperienceYears=newDoctorDto.ExperienceYears,
            ConsultationFee=newDoctorDto.ConsultationFee,
            LicenseNumber=newDoctorDto.LicenseNumber.Trim(),
            DepartmentId=newDoctorDto.DepartmentId,
        };
        context.Doctors.Add(doctor);
        await context.SaveChangesAsync();
        var createdDoctor = await context.Doctors
                            .Include(d => d.Department)
                            .FirstAsync(d => d.Id == doctor.Id);
        return CreatedAtRoute("GetDoctorById",new {id=doctor.Id},createdDoctor.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DoctorDetailsDto>> UpdateDoctorAsync(int id, UpdateDoctorDto updateDoctorDto)
    {
        var doctor = await context.Doctors.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id);
        if (doctor is null) return NotFound();

        if (!await context.Departments.AnyAsync(x => x.Id == updateDoctorDto.DepartmentId))
        {
            return BadRequest();
        }

        if (await context.Doctors.AnyAsync(x => (x.Email == updateDoctorDto.Email ||
                                                x.PhoneNumber == updateDoctorDto.PhoneNumber ||
                                                x.LicenseNumber == updateDoctorDto.LicenseNumber) &&
                                                x.Id != id))
        {
            return Conflict();
        }

        doctor.FirstName = updateDoctorDto.FirstName.Trim();
        doctor.LastName = updateDoctorDto.LastName.Trim();
        doctor.Email = updateDoctorDto.Email.Trim();
        doctor.PhoneNumber = updateDoctorDto.PhoneNumber.Trim();
        doctor.Qualification = updateDoctorDto.Qualification.Trim();
        doctor.Specialization = updateDoctorDto.Specialization.Trim();
        doctor.ExperienceYears = updateDoctorDto.ExperienceYears;
        doctor.ConsultationFee = updateDoctorDto.ConsultationFee;
        doctor.LicenseNumber = updateDoctorDto.LicenseNumber.Trim();
        doctor.DepartmentId = updateDoctorDto.DepartmentId;

        await context.SaveChangesAsync();

        await context.Entry(doctor).Reference(x => x.Department).LoadAsync();

        return Ok(doctor.ToDto());
    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteDoctorAsync(int id)
    {
        var doctor=await context.Doctors.FirstOrDefaultAsync(x=>x.Id==id);
        if(doctor is null) return NotFound();
        context.Doctors.Remove(doctor);
        await context.SaveChangesAsync();
        return NoContent();
    }
}