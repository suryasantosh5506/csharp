using HospitalManagementAPI.Data;
using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.Extensions;
using HospitalManagementAPI.Interfaces;
using HospitalManagementAPI.RequestHelpers;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Services;

public class DoctorAppointmentService(HospitalContext context) : IDoctorAppointmentService
{
    public async Task<PagedList<AppointmentDetailsDto>> GetAllAppointmentsAsync(int doctorId,PaginationParams paginationParams)
    {
        var query=context.Appointments
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .Where(x=>x.DoctorId==doctorId)
            .Select(x=>x.ToDto());

        return await PagedList<AppointmentDetailsDto>.ToPagedList(
            query,
            paginationParams.pageNumber,
            paginationParams.pageSize);
    }

    public async Task<AppointmentDetailsDto?> GetAppointmentByIdAsync(int id,int doctorId)
    {
        var appointment=await context.Appointments
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .FirstOrDefaultAsync(x=>x.Id==id && x.DoctorId==doctorId);

        if(appointment is null) return null;

        return appointment.ToDto();
    }

    public async Task<AppointmentDetailsDto?> ApproveAppointmentAsync(int id,int doctorId)
    {
        var appointment=await context.Appointments
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .FirstOrDefaultAsync(x=>x.Id==id && x.DoctorId==doctorId);

        if(appointment is null) return null;

        if(appointment.Status!="pending") return null;

        appointment.Status="approved";

        await context.SaveChangesAsync();

        return appointment.ToDto();
    }

    public async Task<AppointmentDetailsDto?> RejectAppointmentAsync(int id,int doctorId)
    {
        var appointment=await context.Appointments
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .FirstOrDefaultAsync(x=>x.Id==id && x.DoctorId==doctorId);

        if(appointment is null) return null;

        if(appointment.Status!="pending") return null;

        appointment.Status="reject";

        await context.SaveChangesAsync();

        return appointment.ToDto();
    }

    public async Task<AppointmentDetailsDto?> CompleteAppointmentAsync(int id,int doctorId)
    {
        var appointment=await context.Appointments
            .Include(x=>x.Doctor)
            .Include(x=>x.Patient)
            .FirstOrDefaultAsync(x=>x.Id==id && x.DoctorId==doctorId);

        if(appointment is null) return null;

        if(appointment.Status!="approved") return null;

        appointment.Status="completed";

        await context.SaveChangesAsync();

        return appointment.ToDto();
    }
}