using LearnHubApi.Dtos.Courses;
using LearnHubApi.Extensions;
using LearnHubApi.Interfaces;
using LearnHubApi.RequestHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHubApi.Controllers;

public class CoursesController(ICourseService courseService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses([FromQuery]CourseParams courseParams)
    {
        var courses = await courseService.GetAllAsync(courseParams);
        Response.AddPaginationHeader(courses.paginationMetaData);
        return Ok(courses);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseDto>> GetCourse(int id)
    {
        var course = await courseService.GetByIdAsync(id);
        return Ok(course);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseDto dto)
    {
        var course = await courseService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetCourse),
            new { id = course.Id },
            course
        );
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CourseDto>> UpdateCourse(int id, UpdateCourseDto dto)
    {
        var course = await courseService.UpdateAsync(id, dto);
        return Ok(course);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        await courseService.DeleteAsync(id);
        return NoContent();
    }
}