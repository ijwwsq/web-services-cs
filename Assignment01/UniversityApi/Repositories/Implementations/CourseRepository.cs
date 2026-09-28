using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;

namespace UniversityApi.Repositories.Implementations;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext context;

    public CourseRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<Course>> GetAllAsync(int? teacherId, int? minCredits, string? search)
    {
        var query = context.Courses
            .AsNoTracking()
            .Include(c => c.Teacher)
            .AsQueryable();

        if (teacherId.HasValue)
        {
            query = query.Where(c => c.TeacherId == teacherId.Value);
        }

        if (minCredits.HasValue)
        {
            query = query.Where(c => c.Credits >= minCredits.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = "%" + search + "%";
            query = query.Where(c => EF.Functions.ILike(c.Name, pattern));
        }

        return await query.OrderBy(c => c.Id).ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        return await context.Courses
            .Include(c => c.Teacher)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Courses.AnyAsync(c => c.Id == id);
    }

    public async Task<Course> AddAsync(Course course)
    {
        context.Courses.Add(course);
        await context.SaveChangesAsync();
        return course;
    }

    public async Task UpdateAsync(Course course)
    {
        context.Courses.Update(course);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Course course)
    {
        context.Courses.Remove(course);
        await context.SaveChangesAsync();
    }
}
