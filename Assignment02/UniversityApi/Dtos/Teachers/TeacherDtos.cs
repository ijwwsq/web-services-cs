using System.ComponentModel.DataAnnotations;

namespace UniversityApi.Dtos.Teachers;

public class TeacherDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}

public class TeacherCreateDto
{
    [Required(ErrorMessage = "FirstName is required")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "LastName is required")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email has invalid format")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required")]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;
}

public class TeacherUpdateDto : TeacherCreateDto
{
}
