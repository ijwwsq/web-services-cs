using System.ComponentModel.DataAnnotations;

namespace UniversityApi.Dtos.Courses;

public class CourseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Credits { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CourseCreateDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10")]
    public int Credits { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "TeacherId must be a positive number")]
    public int TeacherId { get; set; }
}

public class CourseUpdateDto : CourseCreateDto
{
}
