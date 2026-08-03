using LearnHubApi.Dtos.Enrollments;
using LearnHubApi.RequestHelpers;

namespace LearnHubApi.Interfaces;

public interface IEnrollmentService
{
    Task<PagedList<EnrollmentDto>> GetMyEnrollmentsAsync(PaginationParams paginationParams);

    Task<EnrollmentDto> EnrollAsync(CreateEnrollmentDto dto);

    Task DeleteAsync(int courseId);
}
