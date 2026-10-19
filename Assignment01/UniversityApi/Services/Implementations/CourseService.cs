using AutoMapper;
using UniversityApi.Common;
using UniversityApi.Dtos.Courses;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;
using UniversityApi.Services.Interfaces;

namespace UniversityApi.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly ICourseRepository courses;
    private readonly ITeacherRepository teachers;
    private readonly IMapper mapper;
    private readonly ILogger<CourseService> logger;

    public CourseService(
        ICourseRepository courses,
        ITeacherRepository teachers,
        IMapper mapper,
        ILogger<CourseService> logger)
    {
        this.courses = courses;
        this.teachers = teachers;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<IReadOnlyList<CourseDto>> GetAllAsync(int? teacherId, int? minCredits, string? search)
    {
        var entities = await courses.GetAllAsync(teacherId, minCredits, search);
        return mapper.Map<IReadOnlyList<CourseDto>>(entities);
    }

    public async Task<CourseDto> GetByIdAsync(int id)
    {
        var course = await courses.GetByIdAsync(id) ?? throw NotFound(id);
        return mapper.Map<CourseDto>(course);
    }

    public async Task<CourseDto> CreateAsync(CourseCreateDto dto)
    {
        await EnsureTeacherExists(dto.TeacherId);

        var course = mapper.Map<Course>(dto);
        course.CreatedAt = DateTime.UtcNow;
        await courses.AddAsync(course);

        logger.LogInformation("Course {CourseId} created for teacher {TeacherId}", course.Id, course.TeacherId);
        return await GetByIdAsync(course.Id);
    }

    public async Task<CourseDto> UpdateAsync(int id, CourseUpdateDto dto)
    {
        var course = await courses.GetByIdAsync(id) ?? throw NotFound(id);
        await EnsureTeacherExists(dto.TeacherId);

        mapper.Map(dto, course);
        await courses.UpdateAsync(course);

        logger.LogInformation("Course {CourseId} updated", id);
        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var course = await courses.GetByIdAsync(id) ?? throw NotFound(id);
        await courses.DeleteAsync(course);
        logger.LogInformation("Course {CourseId} deleted", id);
    }

    private async Task EnsureTeacherExists(int teacherId)
    {
        if (!await teachers.ExistsAsync(teacherId))
        {
            logger.LogWarning("Teacher {TeacherId} not found while saving a course", teacherId);
            throw ApiException.NotFound(ErrorCodes.TeacherNotFound, "Teacher not found");
        }
    }

    private ApiException NotFound(int id)
    {
        logger.LogWarning("Course {CourseId} not found", id);
        return ApiException.NotFound(ErrorCodes.CourseNotFound, "Course not found");
    }
}
