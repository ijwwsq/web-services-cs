using UniversityApi.Dtos.Students;

namespace UniversityApi.Services.Interfaces;

public interface IStudentService
{
    Task<IReadOnlyList<StudentDto>> GetAllAsync();
    Task<StudentDto> GetByIdAsync(int id);
    Task<StudentWithCoursesDto> GetWithCoursesAsync(int id);
    Task<StudentDto> CreateAsync(StudentCreateDto dto);
    Task<StudentDto> UpdateAsync(int id, StudentUpdateDto dto);
    Task DeleteAsync(int id);
}
