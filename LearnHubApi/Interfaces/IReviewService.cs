using LearnHubApi.Dtos.Reviews;

namespace LearnHubApi.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetByCourseAsync(int courseId);

    Task<ReviewDto> CreateAsync(CreateReviewDto dto);

    Task<ReviewDto> UpdateAsync(int id, UpdateReviewDto dto);

    Task DeleteAsync(int id);
}