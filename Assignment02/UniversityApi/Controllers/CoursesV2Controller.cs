using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityApi.Common;
using UniversityApi.Dtos.Courses;
using UniversityApi.Services.Interfaces;

namespace UniversityApi.Controllers;

// Вторая версия отличается только контрактом ответа, бизнес-логика берётся из того же ICourseService.
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/courses")]
[Produces("application/json")]
public class CoursesV2Controller : ControllerBase
{
    private readonly ICourseService courses;

    public CoursesV2Controller(ICourseService courses)
    {
        this.courses = courses;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<IReadOnlyList<CourseDetailsDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? teacherId,
        [FromQuery] int? minCredits,
        [FromQuery] string? search)
    {
        var result = await courses.GetAllDetailedAsync(teacherId, minCredits, search);
        return Ok(ReturnResult<IReadOnlyList<CourseDetailsDto>>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<CourseDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await courses.GetDetailedByIdAsync(id);
        return Ok(ReturnResult<CourseDetailsDto>.Success(result, HttpContext.TraceIdentifier));
    }
}
