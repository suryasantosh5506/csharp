using LearnHubApi.Data;
using LearnHubApi.Dtos.Reviews;
using LearnHubApi.Entities;
using LearnHubApi.Enums;
using LearnHubApi.Exceptions;
using LearnHubApi.Extensions;
using LearnHubApi.Interfaces;
using LearnHubApi.RequestHelpers;
using Microsoft.EntityFrameworkCore;

namespace LearnHubApi.Services;

public class ReviewService(ICurrentUserService userService,AppDbContext context) : IReviewService
{
    public async Task<ReviewDto> CreateAsync(CreateReviewDto dto)
    {
        if (!userService.IsAuthenticated)
        {
            throw new UnauthorizedException("Unauthorized");
        }
        var course = await context.Courses.FirstOrDefaultAsync(x => x.Id == dto.CourseId);

        if (course is null)
        {
            throw new NotFoundException("Course not found.");
        }

        if (!await context.Enrollments.AnyAsync(x =>x.StudentId == userService.UserId && x.CourseId == dto.CourseId))
        {
            throw new ForbiddenException("You must enroll in the course before reviewing it.");
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
            throw new UnauthorizedException("Unauthorized");
        }

        var review=await context.Reviews.Include(x=>x.Author).Include(x=>x.Course).FirstOrDefaultAsync(x=>x.Id==id);

        if(review is null)
        {
            throw new NotFoundException("Review not found");
        }

        if(userService.Role!=UserRole.Admin && userService.UserId != review.AuthorId)
        {
            throw new ForbiddenException("Forbidden");
        }
        context.Reviews.Remove(review);
        await context.SaveChangesAsync();
    }

    public async Task<PagedList<ReviewDto>> GetByCourseAsync(int courseId,PaginationParams paginationParams)
    {
        if(!await context.Courses.AnyAsync(x=>x.Id==courseId))
        {
            throw new NotFoundException("Course not found");
        }
        var query=context.Reviews.Where(x=>x.CourseId==courseId).OrderByDescending(x => x.CreatedAt).Include(x=>x.Course)
                                    .Include(x=>x.Author).Select(x=>x.ToDto());

        var response=await PagedList<ReviewDto>.ToPagedList(query,paginationParams.PageNumber,paginationParams.PageSize);
        return response;
    }

    public async Task<ReviewDto> UpdateAsync(int id, UpdateReviewDto dto)
    {
        if (!userService.IsAuthenticated)
        {
            throw new UnauthorizedException("Unauthorized");
        }

        var review=await context.Reviews.Include(x=>x.Author).Include(x=>x.Course).FirstOrDefaultAsync(x=>x.Id==id);

        if(review is null)
        {
            throw new NotFoundException("Review not found");
        }

        if(userService.Role!=UserRole.Admin && userService.UserId != review.AuthorId)
        {
            throw new ForbiddenException("Forbidden");
        }

        review.Rating=dto.Rating;
        review.Comment=dto.Comment.Trim();

        await context.SaveChangesAsync();
        review = await context.Reviews.Include(x => x.Author).Include(x => x.Course).FirstAsync(x => x.Id == review.Id);
        return review.ToDto();
    }
}