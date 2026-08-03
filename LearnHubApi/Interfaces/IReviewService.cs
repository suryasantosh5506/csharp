using LearnHubApi.Dtos.Reviews;
using LearnHubApi.RequestHelpers;

namespace LearnHubApi.Interfaces;

public interface IReviewService
{
    Task<PagedList<ReviewDto>> GetByCourseAsync(int courseId,PaginationParams paginationParams);

    Task<ReviewDto> CreateAsync(CreateReviewDto dto);

    Task<ReviewDto> UpdateAsync(int id, UpdateReviewDto dto);

    Task DeleteAsync(int id);
}