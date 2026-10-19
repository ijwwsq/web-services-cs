using Microsoft.AspNetCore.Mvc;
using UniversityApi.Common;
using UniversityApi.Dtos.Teachers;
using UniversityApi.Services.Interfaces;

namespace UniversityApi.Controllers;

[ApiController]
[Route("api/teachers")]
[Produces("application/json")]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService teachers;

    public TeachersController(ITeacherService teachers)
    {
        this.teachers = teachers;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ReturnResult<IReadOnlyList<TeacherDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await teachers.GetAllAsync();
        return Ok(ReturnResult<IReadOnlyList<TeacherDto>>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReturnResult<TeacherDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await teachers.GetByIdAsync(id);
        return Ok(ReturnResult<TeacherDto>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReturnResult<TeacherDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(TeacherCreateDto dto)
    {
        var result = await teachers.CreateAsync(dto);
        var body = ReturnResult<TeacherDto>.Success(result, HttpContext.TraceIdentifier, StatusCodes.Status201Created);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, body);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ReturnResult<TeacherDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, TeacherUpdateDto dto)
    {
        var result = await teachers.UpdateAsync(id, dto);
        return Ok(ReturnResult<TeacherDto>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ReturnResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await teachers.DeleteAsync(id);
        return Ok(ReturnResult<string>.Success("Teacher deleted", HttpContext.TraceIdentifier));
    }
}
