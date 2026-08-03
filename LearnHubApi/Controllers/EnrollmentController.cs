using LearnHubApi.Dtos.Enrollments;
using LearnHubApi.Extensions;
using LearnHubApi.Interfaces;
using LearnHubApi.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHubApi.Controllers;

public class EnrollmentsController(IEnrollmentService enrollmentService) : BaseApiController
{
    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetMyEnrollmentsAsync(PaginationParams paginationParams)
    {
        var enrollments=await enrollmentService.GetMyEnrollmentsAsync(paginationParams);
        Response.AddPaginationHeader(enrollments.paginationMetaData);
        return Ok(enrollments);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> EnrollAsync(CreateEnrollmentDto dto)
    {
        var enrollment = await enrollmentService.EnrollAsync(dto);
        return Ok(enrollment);
    }

    [Authorize]
    [HttpDelete("{courseId:int}")]
    public async Task<IActionResult> UnEnrollAsync(int courseId)
    {
        await enrollmentService.DeleteAsync(courseId);
        return NoContent();
    }
}