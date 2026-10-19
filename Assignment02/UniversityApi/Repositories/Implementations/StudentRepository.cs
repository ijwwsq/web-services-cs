using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;

namespace UniversityApi.Repositories.Implementations;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext context;

    public StudentRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<Student>> GetAllAsync()
    {
        return await context.Students
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await context.Students.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student?> GetWithCoursesAsync(int id)
    {
        return await context.Students
            .AsNoTracking()
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c!.Teacher)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Students.AnyAsync(s => s.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email, int? exceptId = null)
    {
        return await context.Students.AnyAsync(s => s.Email == email && (exceptId == null || s.Id != exceptId));
    }

    public async Task<Student> AddAsync(Student student)
    {
        context.Students.Add(student);
        await context.SaveChangesAsync();
        return student;
    }

    public async Task UpdateAsync(Student student)
    {
        context.Students.Update(student);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Student student)
    {
        context.Students.Remove(student);
        await context.SaveChangesAsync();
    }
}
