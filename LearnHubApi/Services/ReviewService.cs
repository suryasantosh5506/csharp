using LearnHubApi.Data;
using LearnHubApi.Dtos.Reviews;
using LearnHubApi.Entities;
using LearnHubApi.Enums;
using LearnHubApi.Extensions;
using LearnHubApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnHubApi.Services;

public class ReviewService(ICurrentUserService userService,AppDbContext context) : IReviewService
{
    public async Task<ReviewDto> CreateAsync(CreateReviewDto dto)
    {
        if (!userService.IsAuthenticated)
        {
            throw new Exception("Unauthorized");
        }
        var course = await context.Courses.FirstOrDefaultAsync(x => x.Id == dto.CourseId);

        if (course is null)
        {
            throw new Exception("Course not found.");
        }

        if (!await context.Enrollments.AnyAsync(x =>x.StudentId == userService.UserId && x.CourseId == dto.CourseId))
        {
            throw new Exception("You must enroll in the course before reviewing it.");
        }

        Review review=new Review()
        {
            Rating=dto.Rating,
            Comment=dto.Comment.Trim(),
            AuthorId=userService.UserId,
            CourseId=dto.CourseId,
            CreatedAt=DateTime.UtcNow
        };
        context.Reviews.Add(review);
        await context.SaveChangesAsync();
        review=await context.Reviews.Include(x=>x.Author).Include(x=>x.Course).FirstAsync(x=>x.Id==review.Id);
        return review.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        if (!userService.IsAuthenticated)
        {
            throw new Exception("Unauthorized");
        }

        var review=await context.Reviews.Include(x=>x.Author).Include(x=>x.Course).FirstOrDefaultAsync(x=>x.Id==id);

        if(review is null)
        {
            throw new Exception("Review not found");
        }

        if(userService.Role!=UserRole.Admin && userService.UserId != review.AuthorId)
        {
            throw new Exception("Forbidden");
        }
        context.Reviews.Remove(review);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ReviewDto>> GetByCourseAsync(int courseId)
    {
        if(!await context.Courses.AnyAsync(x=>x.Id==courseId))
        {
            throw new Exception("Course not found");
        }
        return await context.Reviews.Where(x=>x.CourseId==courseId).OrderByDescending(x => x.CreatedAt).Include(x=>x.Course)
                                    .Include(x=>x.Author).Select(x=>x.ToDto()).ToListAsync();
    }

    public async Task<ReviewDto> UpdateAsync(int id, UpdateReviewDto dto)
    {
        if (!userService.IsAuthenticated)
        {
            throw new Exception("Unauthorized");
        }

        var review=await context.Reviews.Include(x=>x.Author).Include(x=>x.Course).FirstOrDefaultAsync(x=>x.Id==id);

        if(review is null)
        {
            throw new Exception("Review not found");
        }

        if(userService.Role!=UserRole.Admin && userService.UserId != review.AuthorId)
        {
            throw new Exception("Forbidden");
        }

        review.Rating=dto.Rating;
        review.Comment=dto.Comment.Trim();

        await context.SaveChangesAsync();
        review = await context.Reviews.Include(x => x.Author).Include(x => x.Course).FirstAsync(x => x.Id == review.Id);
        return review.ToDto();
    }
}