using UniversityApi.Dtos.Enrollments;

namespace UniversityApi.Services.Interfaces;

public interface IEnrollmentService
{
    Task<IReadOnlyList<EnrollmentDto>> GetAllAsync();
    Task<EnrollmentDto> GetByIdAsync(int id);
    Task<EnrollmentDto> EnrollAsync(EnrollmentCreateDto dto);
    Task<EnrollmentDto> UpdateGradeAsync(int id, EnrollmentGradeUpdateDto dto);
    Task DeleteAsync(int id);
}
