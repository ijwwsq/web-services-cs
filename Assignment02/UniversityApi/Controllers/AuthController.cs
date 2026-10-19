using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityApi.Common;
using UniversityApi.Dtos.Auth;
using UniversityApi.Services.Interfaces;

namespace UniversityApi.Controllers;

[ApiController]
[ApiVersionNeutral]
[Route("api/auth")]
[Produces("application/json")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IAuthService auth;

    public AuthController(IAuthService auth)
    {
        this.auth = auth;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<CurrentUserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await auth.RegisterAsync(dto);
        var body = ReturnResult<CurrentUserDto>.Success(
            result,
            HttpContext.TraceIdentifier,
            StatusCodes.Status201Created);
        return StatusCode(StatusCodes.Status201Created, body);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReturnResult<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await auth.LoginAsync(dto);
        return Ok(ReturnResult<AuthResponseDto>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ReturnResult<CurrentUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me()
    {
        var result = await auth.GetCurrentUserAsync();
        return Ok(ReturnResult<CurrentUserDto>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpGet("users")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ReturnResult<IReadOnlyList<CurrentUserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsers()
    {
        var result = await auth.GetUsersAsync();
        return Ok(ReturnResult<IReadOnlyList<CurrentUserDto>>.Success(result, HttpContext.TraceIdentifier));
    }

    [HttpPost("users/{userId}/roles")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ReturnResult<CurrentUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ReturnResult<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRole(string userId, AssignRoleDto dto)
    {
        var result = await auth.AssignRoleAsync(userId, dto);
        return Ok(ReturnResult<CurrentUserDto>.Success(result, HttpContext.TraceIdentifier));
    }
}
