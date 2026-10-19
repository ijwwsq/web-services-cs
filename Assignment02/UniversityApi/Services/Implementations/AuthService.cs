using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityApi.Common;
using UniversityApi.Dtos.Auth;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;
using UniversityApi.Services.Interfaces;

namespace UniversityApi.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly ITokenService tokenService;
    private readonly ICurrentUser currentUser;
    private readonly ITeacherRepository teachers;
    private readonly IStudentRepository students;
    private readonly ILogger<AuthService> logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ICurrentUser currentUser,
        ITeacherRepository teachers,
        IStudentRepository students,
        ILogger<AuthService> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.tokenService = tokenService;
        this.currentUser = currentUser;
        this.teachers = teachers;
        this.students = students;
        this.logger = logger;
    }

    public async Task<CurrentUserDto> RegisterAsync(RegisterDto dto)
    {
        if (await userManager.FindByNameAsync(dto.UserName) != null)
        {
            logger.LogWarning("Registration rejected, user name {UserName} is taken", dto.UserName);
            throw ApiException.Conflict(ErrorCodes.UserAlreadyExists, "User with this user name already exists");
        }

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var message = string.Join("; ", result.Errors.Select(error => error.Description));
            logger.LogWarning("Registration failed for {UserName}: {Reason}", dto.UserName, message);
            throw ApiException.BadRequest(ErrorCodes.ValidationError, message);
        }

        // Регистрация никогда не выдаёт административные права, только базовую роль.
        await userManager.AddToRoleAsync(user, Roles.Student);

        logger.LogInformation("User {UserName} registered with role {Role}", user.UserName, Roles.Student);
        return await MapAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await userManager.FindByNameAsync(dto.UserName);
        if (user == null)
        {
            logger.LogWarning("Login failed, user {UserName} not found", dto.UserName);
            throw ApiException.Unauthorized(ErrorCodes.InvalidCredentials, "Invalid user name or password");
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            logger.LogWarning("Login blocked, account {UserName} is locked out", dto.UserName);
            throw ApiException.Unauthorized(ErrorCodes.AccountLocked, "Account is locked out, try again later");
        }

        if (!result.Succeeded)
        {
            logger.LogWarning("Login failed, wrong password for {UserName}", dto.UserName);
            throw ApiException.Unauthorized(ErrorCodes.InvalidCredentials, "Invalid user name or password");
        }

        var roles = await userManager.GetRolesAsync(user);
        var token = tokenService.CreateAccessToken(user, roles);

        logger.LogInformation("User {UserName} logged in, token expires at {ExpiresAt}", user.UserName, token.ExpiresAt);

        return new AuthResponseDto
        {
            AccessToken = token.AccessToken,
            ExpiresAt = token.ExpiresAt,
            UserName = user.UserName ?? string.Empty,
            Roles = roles
        };
    }

    public async Task<CurrentUserDto> GetCurrentUserAsync()
    {
        var user = await FindUserAsync(currentUser.UserId);
        return await MapAsync(user);
    }

    public async Task<IReadOnlyList<CurrentUserDto>> GetUsersAsync()
    {
        var users = await userManager.Users.OrderBy(u => u.UserName).ToListAsync();

        var result = new List<CurrentUserDto>();
        foreach (var user in users)
        {
            result.Add(await MapAsync(user));
        }

        return result;
    }

    public async Task<CurrentUserDto> AssignRoleAsync(string userId, AssignRoleDto dto)
    {
        if (!Roles.All.Contains(dto.Role))
        {
            throw ApiException.NotFound(ErrorCodes.RoleNotFound, "Unknown role");
        }

        var user = await FindUserAsync(userId);

        if (dto.TeacherId.HasValue && !await teachers.ExistsAsync(dto.TeacherId.Value))
        {
            throw ApiException.NotFound(ErrorCodes.TeacherNotFound, "Teacher not found");
        }

        if (dto.StudentId.HasValue && !await students.ExistsAsync(dto.StudentId.Value))
        {
            throw ApiException.NotFound(ErrorCodes.StudentNotFound, "Student not found");
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        await userManager.AddToRoleAsync(user, dto.Role);

        user.TeacherId = dto.Role == Roles.Teacher ? dto.TeacherId : null;
        user.StudentId = dto.Role == Roles.Student ? dto.StudentId : null;
        await userManager.UpdateAsync(user);

        logger.LogInformation("User {UserName} got role {Role}", user.UserName, dto.Role);
        return await MapAsync(user);
    }

    private async Task<ApplicationUser> FindUserAsync(string? userId)
    {
        var user = userId == null ? null : await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw ApiException.NotFound(ErrorCodes.UserNotFound, "User not found");
        }

        return user;
    }

    private async Task<CurrentUserDto> MapAsync(ApplicationUser user) => new()
    {
        Id = user.Id,
        UserName = user.UserName ?? string.Empty,
        Email = user.Email ?? string.Empty,
        Roles = await userManager.GetRolesAsync(user),
        TeacherId = user.TeacherId,
        StudentId = user.StudentId
    };
}
