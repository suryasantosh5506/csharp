using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles ="Admin")]
public class AdminAppointmentController(HospitalContext context):BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<AppointmentDetailsDto>>> GetAllAppointmentsAsync([FromQuery]PaginationParams paginationParams)
    {
        var query=context.Appointments.Include(x=>x.Patient).Include(x=>x.Doctor).Select(x=>x.ToDto());
        var appointments=await PagedList<AppointmentDetailsDto>.ToPagedList(query,paginationParams.pageNumber,paginationParams.pageSize);
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
    public async Task<ActionResult<PagedList
    <AppointmentDetailsDto>>> GetPendingAppointmentsAsync([FromQuery] PaginationParams paginationParams)
    {
        var query=context.Appointments.Include(x=>x.Patient).Include(x=>x.Doctor).Where(x=>x.Status=="pending")
                                                    .Select(x=>x.ToDto());
        var appointments=await PagedList<AppointmentDetailsDto>.ToPagedList(query,paginationParams.pageNumber,paginationParams.pageSize);
        return Ok(appointments);
    }
}