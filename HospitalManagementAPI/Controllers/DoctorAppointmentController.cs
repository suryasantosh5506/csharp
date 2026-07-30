using System.Security.Claims;
using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles = "Doctor")]
public class DoctorAppointmentController(HospitalContext context) : BaseApiController
{
    private async Task<Doctor?> GetCurrentDoctorAsync()
    {
        var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await context.Doctors.FirstOrDefaultAsync(x=>x.UserId==userId);
    }

    [HttpGet]
    public async Task<ActionResult<List<AppointmentDetailsDto>>> GetAllAppointmentsAsync()
    {
        var doctor=await GetCurrentDoctorAsync();
        if(doctor is null) return Unauthorized();
        var appointments=await context.Appointments.Include(x=>x.Doctor).Include(x=>x.Patient).Where(x=>x.DoctorId==doctor.Id).Select(x=>x.ToDto()).ToListAsync();
        return Ok(appointments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentDetailsDto>> GetAppointmentByIdAsync(int id)
    {
        var doctor=await GetCurrentDoctorAsync();
        if(doctor is null) return Unauthorized();
        var appointment=await context.Appointments.Include(x=>x.Patient).Include(x=>x.Doctor).FirstOrDefaultAsync(x=>x.Id==id && x.DoctorId==doctor.Id);
        if(appointment is null) return NotFound();
        return Ok(appointment.ToDto());
    }
}