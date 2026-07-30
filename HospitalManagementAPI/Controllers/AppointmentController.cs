using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Controllers;

public class AppointmentController(HospitalContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<AppointmentDetailsDto>>> GetAllAppointmentsAsync()
    {
        var appointments=await context.Appointments.Include(x=>x.Doctor).Include(x=>x.Patient).Select(x=>x.ToDto()).ToListAsync();
        return Ok(appointments);
    }

    [HttpGet("{id:int}",Name ="GetAppointmentById")]
    public async Task<ActionResult<AppointmentDetailsDto>> GetAppointmentById(int id)
    {
        var appointment=await context.Appointments.Include(x=>x.Doctor).Include(x=>x.Patient).FirstOrDefaultAsync(x=>x.Id==id);
        if(appointment is null) return NotFound();
        return Ok(appointment.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDetailsDto>> CreateAppointmentAsync(CreateAppointmentDto newAppointment)
    {
        if(! await context.Patients.AnyAsync(x=>x.Id==newAppointment.PatientId)) return BadRequest();
        if(! await context.Doctors.AnyAsync(x=>x.Id==newAppointment.DoctorId)) return BadRequest();

        if(newAppointment.AppointmentDate < DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest("Appointment date cannot be in the past.");
        }

        bool doctorsConflict=await context.Appointments.Where(x=>x.DoctorId==newAppointment.DoctorId)
                                .AnyAsync(x=>x.AppointmentDate==newAppointment.AppointmentDate && x.AppointmentTime==newAppointment.AppointmentTime);
        if(doctorsConflict) return Conflict("Appointment was not possible because doctor was busy");

        bool patientsConflict=await context.Appointments.Where(x=>x.PatientId==newAppointment.PatientId)
                                .AnyAsync(x=>x.AppointmentDate==newAppointment.AppointmentDate && x.AppointmentTime==newAppointment.AppointmentTime);
        if(patientsConflict) return Conflict("Appointment was not possible because there is another appointment scheduled for you at that time");

        Appointment appointment = new()
        {
            DoctorId=newAppointment.DoctorId,
            PatientId=newAppointment.PatientId,
            AppointmentDate=newAppointment.AppointmentDate,
            AppointmentTime=newAppointment.AppointmentTime,
            Reason=newAppointment.Reason,
            Status="Pending",
            CreatedAt=DateTime.UtcNow
        };

        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        appointment=await context.Appointments.Include(x=>x.Doctor)
                                              .Include(x=>x.Patient)
                                              .FirstAsync(x=>x.Id==appointment.Id);
        return CreatedAtRoute("GetAppointmentById",new {id=appointment.Id},appointment.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AppointmentDetailsDto>> UpdateAppointmentAsync(int id,UpdateAppointmentDto updateAppointmentDto)
    {
        var appointment=await context.Appointments.Include(x=>x.Doctor).Include(x=>x.Patient).FirstOrDefaultAsync(x=>x.Id==id);
        if(appointment is null)  return NotFound();

        if(updateAppointmentDto.AppointmentDate < DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest("Appointment date cannot be in the past.");
        }

        bool doctorsConflict=await context.Appointments.Where(x=>x.DoctorId==appointment.DoctorId)
                                .AnyAsync(x=>x.AppointmentDate==updateAppointmentDto.AppointmentDate && x.AppointmentTime==updateAppointmentDto.AppointmentTime && x.Id!=id);
        if(doctorsConflict) return Conflict("Appointment was not possible because doctor was busy");

        bool patientsConflict=await context.Appointments.Where(x=>x.PatientId==appointment.PatientId)
                                .AnyAsync(x=>x.AppointmentDate==updateAppointmentDto.AppointmentDate && x.AppointmentTime==updateAppointmentDto.AppointmentTime && x.Id!=id);
        if(patientsConflict) return Conflict("Appointment was not possible because there is another appointment scheduled for you at that time");

        appointment.AppointmentDate=updateAppointmentDto.AppointmentDate;
        appointment.AppointmentTime=updateAppointmentDto.AppointmentTime;
        appointment.Reason=updateAppointmentDto.Reason.Trim();
        appointment.Status=updateAppointmentDto.Status.Trim();

        await context.SaveChangesAsync();
        return Ok(appointment.ToDto());
    }
}