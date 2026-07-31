using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.Entities;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Services;

public class AppointmentService(HospitalContext context) : IAppointmentService
{
    public async Task<PagedList<AppointmentDetailsDto>> GetAllAppointmentsAsync(int patientId, PaginationParams paginationParams)
    {
        var query=context.Appointments
            .Where(x=>x.PatientId==patientId)
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .Select(x=>x.ToDto());

        var appointments=await PagedList<AppointmentDetailsDto>.ToPagedList(
            query,
            paginationParams.pageNumber,
            paginationParams.pageSize);

        return appointments;
    }

    public async Task<AppointmentDetailsDto?> GetAppointmentByIdAsync(int id,int patientId)
    {
        var appointment=await context.Appointments
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .FirstOrDefaultAsync(x=>x.Id==id && x.PatientId==patientId);

        if(appointment is null) return null;

        return appointment.ToDto();
    }

    public async Task<AppointmentDetailsDto> CreateAppointmentAsync(int patientId,CreateAppointmentDto createAppointmentDto)
    {
        Appointment appointment=new()
        {
            DoctorId=createAppointmentDto.DoctorId,
            PatientId=patientId,
            AppointmentDate=createAppointmentDto.AppointmentDate,
            AppointmentTime=createAppointmentDto.AppointmentTime,
            Reason=createAppointmentDto.Reason.Trim(),
            Status="Pending",
            CreatedAt=DateTime.UtcNow
        };

        context.Appointments.Add(appointment);

        await context.SaveChangesAsync();

        appointment=await context.Appointments
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .FirstAsync(x=>x.Id==appointment.Id);

        return appointment.ToDto();
    }

    public async Task<AppointmentDetailsDto?> UpdateAppointmentAsync(int id,int patientId,UpdateAppointmentDto updateAppointmentDto)
    {
        var appointment=await context.Appointments
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .FirstOrDefaultAsync(x=>x.Id==id && x.PatientId==patientId);

        if(appointment is null) return null;

        appointment.AppointmentDate=updateAppointmentDto.AppointmentDate;
        appointment.AppointmentTime=updateAppointmentDto.AppointmentTime;
        appointment.Reason=updateAppointmentDto.Reason.Trim();
        appointment.Status=updateAppointmentDto.Status.Trim();

        await context.SaveChangesAsync();

        return appointment.ToDto();
    }

    public async Task<bool> DeleteAppointmentAsync(int id,int patientId)
    {
        var appointment=await context.Appointments
            .FirstOrDefaultAsync(x=>x.Id==id && x.PatientId==patientId);

        if(appointment is null) return false;

        context.Appointments.Remove(appointment);

        await context.SaveChangesAsync();

        return true;
    }
}