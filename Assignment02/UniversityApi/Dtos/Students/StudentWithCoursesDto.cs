namespace UniversityApi.Dtos.Students;

public class StudentWithCoursesDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<StudentCourseDto> Courses { get; set; } = new();
}

public class StudentCourseDto
{
    public int CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public int? Grade { get; set; }
}
