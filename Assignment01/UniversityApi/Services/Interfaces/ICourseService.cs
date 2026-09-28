using UniversityApi.Dtos.Courses;

namespace UniversityApi.Services.Interfaces;

public interface ICourseService
{
    Task<IReadOnlyList<CourseDto>> GetAllAsync(int? teacherId, int? minCredits, string? search);
    Task<CourseDto> GetByIdAsync(int id);
    Task<CourseDto> CreateAsync(CourseCreateDto dto);
    Task<CourseDto> UpdateAsync(int id, CourseUpdateDto dto);
    Task DeleteAsync(int id);
}
