using LearnHubApi.Dtos.Enrollments;

namespace LearnHubApi.Interfaces;

public interface IEnrollmentService
{
    Task<IEnumerable<EnrollmentDto>> GetMyEnrollmentsAsync();

    Task<EnrollmentDto> EnrollAsync(CreateEnrollmentDto dto);

    Task DeleteAsync(int courseId);
}
