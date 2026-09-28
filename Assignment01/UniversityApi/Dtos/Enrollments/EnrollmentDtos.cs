using System.ComponentModel.DataAnnotations;

namespace UniversityApi.Dtos.Enrollments;

public class EnrollmentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public int? Grade { get; set; }
}

public class EnrollmentCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "StudentId must be a positive number")]
    public int StudentId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CourseId must be a positive number")]
    public int CourseId { get; set; }
}

public class EnrollmentGradeUpdateDto
{
    [Range(0, 100, ErrorMessage = "Grade must be between 0 and 100")]
    public int Grade { get; set; }
}
