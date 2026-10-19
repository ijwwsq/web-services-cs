using UniversityApi.Models;

namespace UniversityApi.Repositories.Interfaces;

public interface ITeacherRepository
{
    Task<IReadOnlyList<Teacher>> GetAllAsync();
    Task<Teacher?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? exceptId = null);
    Task<Teacher> AddAsync(Teacher teacher);
    Task UpdateAsync(Teacher teacher);
    Task DeleteAsync(Teacher teacher);
}
