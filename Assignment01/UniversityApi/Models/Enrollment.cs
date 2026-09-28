namespace UniversityApi.Models;

public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public int? Grade { get; set; }
}
