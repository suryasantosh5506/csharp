using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles ="Admin")]
public class AdminAppointmentController(HospitalContext context):BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<AppointmentDetailsDto>>> GetAllAppointmentsAsync()
    {
        var appointments=await context.Appointments.Include(x=>x.Patient).Include(x=>x.Doctor).Select(x=>x.ToDto()).ToListAsync();
        return Ok(appointments);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentDetailsDto>> GetAppointmentByIdAsync(int id)
    {
        var appointment=await context.Appointments.Include(x=>x.Patient).Include(x=>x.Doctor).FirstOrDefaultAsync(x=>x.Id==id);
        if(appointment is null) return NotFound();
        return Ok(appointment.ToDto());
    }

    [HttpGet("pending")]
    public async Task<ActionResult<List<AppointmentDetailsDto>>> GetPendingAppointmentsAsync()
    {
        var appointments=await context.Appointments.Include(x=>x.Patient).Include(x=>x.Doctor).Where(x=>x.Status=="pending")
                                                    .Select(x=>x.ToDto()).ToListAsync();
        return Ok(appointments);
    }
}