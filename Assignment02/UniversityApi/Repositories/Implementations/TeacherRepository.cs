using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;

namespace UniversityApi.Repositories.Implementations;

public class TeacherRepository : ITeacherRepository
{
    private readonly ApplicationDbContext context;

    public TeacherRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<Teacher>> GetAllAsync()
    {
        return await context.Teachers
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToListAsync();
    }

    public async Task<Teacher?> GetByIdAsync(int id)
    {
        return await context.Teachers.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Teachers.AnyAsync(t => t.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email, int? exceptId = null)
    {
        return await context.Teachers.AnyAsync(t => t.Email == email && (exceptId == null || t.Id != exceptId));
    }

    public async Task<Teacher> AddAsync(Teacher teacher)
    {
        context.Teachers.Add(teacher);
        await context.SaveChangesAsync();
        return teacher;
    }

    public async Task UpdateAsync(Teacher teacher)
    {
        context.Teachers.Update(teacher);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Teacher teacher)
    {
        context.Teachers.Remove(teacher);
        await context.SaveChangesAsync();
    }
}
