using Microsoft.AspNetCore.Mvc;
using UniversityApi.Common;
using UniversityApi.Dtos.Enrollments;
using UniversityApi.Services.Interfaces;

namespace UniversityApi.Controllers;

[ApiController]
[Route("api/enrollments")]
[Produces("application/json")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService enrollments;

    public EnrollmentsController(IEnrollmentService enrollments)
    {
        this.enrollments = enrollments;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ReturnResult<IReadOnlyList<EnrollmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await enrollments.GetAllAsync();
        return Ok(ReturnResult<IReadOnlyList<EnrollmentDto>>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReturnResult<EnrollmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await enrollments.GetByIdAsync(id);
        return Ok(ReturnResult<EnrollmentDto>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReturnResult<EnrollmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Enroll(EnrollmentCreateDto dto)
    {
        var result = await enrollments.EnrollAsync(dto);
        var body = ReturnResult<EnrollmentDto>.Success(
            result,
            HttpContext.TraceIdentifier,
            StatusCodes.Status201Created);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, body);
    }

    [HttpPut("{id:int}/grade")]
    [ProducesResponseType(typeof(ReturnResult<EnrollmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGrade(int id, EnrollmentGradeUpdateDto dto)
    {
        var result = await enrollments.UpdateGradeAsync(id, dto);
        return Ok(ReturnResult<EnrollmentDto>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ReturnResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await enrollments.DeleteAsync(id);
        return Ok(ReturnResult<string>.Success("Student removed from the course", HttpContext.TraceIdentifier));
    }
}
