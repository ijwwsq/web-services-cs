using UniversityApi.Dtos.Auth;

namespace UniversityApi.Services.Interfaces;

public interface IAuthService
{
    Task<CurrentUserDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<CurrentUserDto> GetCurrentUserAsync();
    Task<IReadOnlyList<CurrentUserDto>> GetUsersAsync();
    Task<CurrentUserDto> AssignRoleAsync(string userId, AssignRoleDto dto);
}
