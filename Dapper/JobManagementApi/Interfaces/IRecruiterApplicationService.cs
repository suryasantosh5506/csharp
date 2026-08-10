using JobManagementApi.Dtos.RecruiterApplication;

namespace JobManagementApi.Interfaces;

public interface IRecruiterApplicationService
{
    Task<RecruiterApplicationDto> CreateApplication(CreateRecruiterApplicationDto dto);

    Task<IEnumerable<RecruiterApplicationDto>> GetMyApplications();

    Task<IEnumerable<RecruiterApplicationDto>> GetApplications();

    Task<RecruiterApplicationDto> GetApplicationById(int id);

    Task<bool> UpdateApplication(int id,UpdateRecruiterApplicationDto dto);

    Task<bool> DeleteApplication(int id);
}