using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.RequestHelpers;

namespace HospitalManagementAPI.Interfaces;

public interface IDoctorAppointmentService
{
    Task<PagedList<AppointmentDetailsDto>> GetAllAppointmentsAsync(int doctorId,PaginationParams paginationParams);

    Task<AppointmentDetailsDto?> GetAppointmentByIdAsync(int id,int doctorId);

    Task<AppointmentDetailsDto?> ApproveAppointmentAsync(int id,int doctorId);

    Task<AppointmentDetailsDto?> RejectAppointmentAsync(int id,int doctorId);

    Task<AppointmentDetailsDto?> CompleteAppointmentAsync(int id,int doctorId);
}