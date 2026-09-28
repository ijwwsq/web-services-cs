using System.ComponentModel.DataAnnotations;

namespace UniversityApi.Dtos.Students;

public class StudentUpdateDto
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

    [Required(ErrorMessage = "BirthDate is required")]
    public DateOnly BirthDate { get; set; }
}
