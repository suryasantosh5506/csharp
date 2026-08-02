using LearnHubApi.Dtos.Reviews;
using LearnHubApi.Entities;

namespace LearnHubApi.Extensions;

public static class ReviewExtension
{
    public static ReviewDto ToDto(this Review review)
    {
        return new ReviewDto(review.Id,review.Rating,review.Comment,review.AuthorId,review.Author.FirstName+review.Author.LastName,
                                review.CourseId,review.Course.Title,review.CreatedAt);
    }
}