using UniversityApi.Models;

namespace UniversityApi.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    Task<IReadOnlyList<Enrollment>> GetAllAsync();
    Task<Enrollment?> GetByIdAsync(int id);
    Task<bool> ExistsForStudentAndCourseAsync(int studentId, int courseId);
    Task<Enrollment> AddAsync(Enrollment enrollment);
    Task UpdateAsync(Enrollment enrollment);
    Task DeleteAsync(Enrollment enrollment);
}
