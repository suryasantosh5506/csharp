using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.enums;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using HospitalManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementAPI.Controllers;

[Authorize(Roles =nameof(UserRole.Admin))]
public class AdminAppointmentController(IAdminAppointmentService adminAppointmentService):BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<AppointmentDetailsDto>>> GetAllAppointmentsAsync([FromQuery]PaginationParams paginationParams)
    {
        var appointments=await adminAppointmentService.GetAllAppointmentsForAdminAsync(paginationParams);

        return Ok(appointments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentDetailsDto>> GetAppointmentByIdAsync(int id)
    {
        var appointment=await adminAppointmentService.GetAppointmentByIdForAdminAsync(id);

        if(appointment is null) return NotFound();

        return Ok(appointment);
    }

    [HttpGet("pending")]
    public async Task<ActionResult<PagedList<AppointmentDetailsDto>>> GetPendingAppointmentsAsync([FromQuery]PaginationParams paginationParams)
    {
        var appointments=await adminAppointmentService.GetPendingAppointmentsAsync(paginationParams);

        return Ok(appointments);
    }
}