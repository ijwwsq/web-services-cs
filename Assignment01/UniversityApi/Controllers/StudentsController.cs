using Microsoft.AspNetCore.Mvc;
using UniversityApi.Common;
using UniversityApi.Dtos.Students;
using UniversityApi.Services.Interfaces;

namespace UniversityApi.Controllers;

[ApiController]
[Route("api/students")]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService students;

    public StudentsController(IStudentService students)
    {
        this.students = students;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ReturnResult<IReadOnlyList<StudentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await students.GetAllAsync();
        return Ok(ReturnResult<IReadOnlyList<StudentDto>>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReturnResult<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await students.GetByIdAsync(id);
        return Ok(ReturnResult<StudentDto>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}/courses")]
    [ProducesResponseType(typeof(ReturnResult<StudentWithCoursesDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWithCourses(int id)
    {
        var result = await students.GetWithCoursesAsync(id);
        return Ok(ReturnResult<StudentWithCoursesDto>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReturnResult<StudentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(StudentCreateDto dto)
    {
        var result = await students.CreateAsync(dto);
        var body = ReturnResult<StudentDto>.Success(result, HttpContext.TraceIdentifier, StatusCodes.Status201Created);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, body);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ReturnResult<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, StudentUpdateDto dto)
    {
        var result = await students.UpdateAsync(id, dto);
        return Ok(ReturnResult<StudentDto>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ReturnResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await students.DeleteAsync(id);
        return Ok(ReturnResult<string>.Success("Student deleted", HttpContext.TraceIdentifier));
    }
}
