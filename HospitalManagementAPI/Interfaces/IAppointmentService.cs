using HospitalManagementAPI.Dtos.Appointment;
using HospitalManagementAPI.RequestHelpers;

namespace HospitalManagementAPI.Interfaces;

public interface IAppointmentService
{
    Task<PagedList<AppointmentDetailsDto>> GetAllAppointmentsAsync(int patientId, PaginationParams paginationParams);

    Task<AppointmentDetailsDto?> GetAppointmentByIdAsync(int id,int patientId);

    Task<AppointmentDetailsDto> CreateAppointmentAsync(int patientId,CreateAppointmentDto createAppointmentDto);

    Task<AppointmentDetailsDto?> UpdateAppointmentAsync(int id,int patientId,UpdateAppointmentDto updateAppointmentDto);

    Task<bool> DeleteAppointmentAsync(int id,int patientId);
}