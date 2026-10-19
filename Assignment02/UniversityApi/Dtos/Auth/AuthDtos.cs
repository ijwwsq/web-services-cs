using System.ComponentModel.DataAnnotations;

namespace UniversityApi.Dtos.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "UserName is required")]
    [MinLength(3, ErrorMessage = "UserName must be at least 3 characters long")]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email has invalid format")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    [Required(ErrorMessage = "UserName is required")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
}

public class CurrentUserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public int? TeacherId { get; set; }
    public int? StudentId { get; set; }
}

public class AssignRoleDto
{
    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; } = string.Empty;

    public int? TeacherId { get; set; }

    public int? StudentId { get; set; }
}
