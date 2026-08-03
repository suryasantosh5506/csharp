using LearnHubApi.Dtos.Reviews;
using LearnHubApi.Extensions;
using LearnHubApi.Interfaces;
using LearnHubApi.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHubApi.Controllers;

public class ReviewsController(IReviewService reviewService) : BaseApiController
{
    [HttpGet("course/{courseId:int}")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByCourseAsync(int courseId,PaginationParams paginationParams)
    {
        var reviews=await reviewService.GetByCourseAsync(courseId,paginationParams);
        Response.AddPaginationHeader(reviews.paginationMetaData);
        return Ok(reviews);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ReviewDto>> CreateReviewAsync(CreateReviewDto dto)
    {
        var review = await reviewService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetReviewsByCourseAsync),
            new { courseId = review.CourseId },
            review);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReviewDto>> UpdateReviewAsync(
        int id,
        UpdateReviewDto dto)
    {
        return Ok(await reviewService.UpdateAsync(id, dto));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteReviewAsync(int id)
    {
        await reviewService.DeleteAsync(id);
        return NoContent();
    }
}