using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityApi.Common;
using UniversityApi.Dtos.Courses;
using UniversityApi.Services.Interfaces;

namespace UniversityApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/courses")]
[Route("api/courses")]
[Produces("application/json")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService courses;

    public CoursesController(ICourseService courses)
    {
        this.courses = courses;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<IReadOnlyList<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? teacherId,
        [FromQuery] int? minCredits,
        [FromQuery] string? search)
    {
        var result = await courses.GetAllAsync(teacherId, minCredits, search);
        return Ok(ReturnResult<IReadOnlyList<CourseDto>>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await courses.GetByIdAsync(id);
        return Ok(ReturnResult<CourseDto>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ReturnResult<CourseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(CourseCreateDto dto)
    {
        var result = await courses.CreateAsync(dto);
        var body = ReturnResult<CourseDto>.Success(result, HttpContext.TraceIdentifier, StatusCodes.Status201Created);
        Response.Headers.Location = "/api/v1/courses/" + result.Id;
        return StatusCode(StatusCodes.Status201Created, body);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ReturnResult<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, CourseUpdateDto dto)
    {
        var result = await courses.UpdateAsync(id, dto);
        return Ok(ReturnResult<CourseDto>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ReturnResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await courses.DeleteAsync(id);
        return Ok(ReturnResult<string>.Success("Course deleted", HttpContext.TraceIdentifier));
    }
}
