using AutoMapper;
using UniversityApi.Common;
using UniversityApi.Dtos.Enrollments;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;
using UniversityApi.Services.Interfaces;

namespace UniversityApi.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository enrollments;
    private readonly IStudentRepository students;
    private readonly ICourseRepository courses;
    private readonly IMapper mapper;
    private readonly ILogger<EnrollmentService> logger;

    public EnrollmentService(
        IEnrollmentRepository enrollments,
        IStudentRepository students,
        ICourseRepository courses,
        IMapper mapper,
        ILogger<EnrollmentService> logger)
    {
        this.enrollments = enrollments;
        this.students = students;
        this.courses = courses;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<IReadOnlyList<EnrollmentDto>> GetAllAsync()
    {
        var entities = await enrollments.GetAllAsync();
        return mapper.Map<IReadOnlyList<EnrollmentDto>>(entities);
    }

    public async Task<EnrollmentDto> GetByIdAsync(int id)
    {
        var enrollment = await enrollments.GetByIdAsync(id) ?? throw NotFound(id);
        return mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<EnrollmentDto> EnrollAsync(EnrollmentCreateDto dto)
    {
        if (!await students.ExistsAsync(dto.StudentId))
        {
            logger.LogWarning("Student {StudentId} not found while enrolling", dto.StudentId);
            throw ApiException.NotFound(ErrorCodes.StudentNotFound, "Student not found");
        }

        if (!await courses.ExistsAsync(dto.CourseId))
        {
            logger.LogWarning("Course {CourseId} not found while enrolling", dto.CourseId);
            throw ApiException.NotFound(ErrorCodes.CourseNotFound, "Course not found");
        }

        if (await enrollments.ExistsForStudentAndCourseAsync(dto.StudentId, dto.CourseId))
        {
            logger.LogWarning(
                "Student {StudentId} is already enrolled in course {CourseId}",
                dto.StudentId,
                dto.CourseId);
            throw ApiException.Conflict(
                ErrorCodes.DuplicateEnrollment,
                "Student is already enrolled in this course");
        }

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            EnrollmentDate = DateTime.UtcNow
        };

        await enrollments.AddAsync(enrollment);
        logger.LogInformation(
            "Student {StudentId} enrolled in course {CourseId}, enrollment {EnrollmentId}",
            dto.StudentId,
            dto.CourseId,
            enrollment.Id);

        return await GetByIdAsync(enrollment.Id);
    }

    public async Task<EnrollmentDto> UpdateGradeAsync(int id, EnrollmentGradeUpdateDto dto)
    {
        var enrollment = await enrollments.GetByIdAsync(id) ?? throw NotFound(id);

        enrollment.Grade = dto.Grade;
        await enrollments.UpdateAsync(enrollment);

        logger.LogInformation("Grade {Grade} set for enrollment {EnrollmentId}", dto.Grade, id);
        return mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task DeleteAsync(int id)
    {
        var enrollment = await enrollments.GetByIdAsync(id) ?? throw NotFound(id);
        await enrollments.DeleteAsync(enrollment);
        logger.LogInformation("Enrollment {EnrollmentId} deleted", id);
    }

    private ApiException NotFound(int id)
    {
        logger.LogWarning("Enrollment {EnrollmentId} not found", id);
        return ApiException.NotFound(ErrorCodes.EnrollmentNotFound, "Enrollment not found");
    }
}
