using LearnHubApi.Dtos.Lessons;
using LearnHubApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHubApi.Controllers;

public class LessonsController(ILessonService lessonService) : BaseApiController
{
    [HttpGet("module/{moduleId:int}")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessonsByModuleAsync(int moduleId)
    {
        return Ok(await lessonService.GetByModuleAsync(moduleId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LessonDto>> GetLessonByIdAsync(int id)
    {
        return Ok(await lessonService.GetByIdAsync(id));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<LessonDto>> CreateLessonAsync([FromForm] CreateLessonDto lessonDto)
    {
        var lesson = await lessonService.CreateAsync(lessonDto);

        return CreatedAtAction(
            nameof(GetLessonByIdAsync),
            new { id = lesson.Id },
            lesson);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<LessonDto>> UpdateLessonAsync(
        int id,
        [FromForm] UpdateLessonDto updateDto)
    {
        return Ok(await lessonService.UpdateAsync(id, updateDto));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteLessonAsync(int id)
    {
        await lessonService.DeleteAsync(id);
        return NoContent();
    }
}