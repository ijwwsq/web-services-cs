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
    private readonly ICurrentUser currentUser;
    private readonly ILogger<EnrollmentService> logger;

    public EnrollmentService(
        IEnrollmentRepository enrollments,
        IStudentRepository students,
        ICourseRepository courses,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<EnrollmentService> logger)
    {
        this.enrollments = enrollments;
        this.students = students;
        this.courses = courses;
        this.mapper = mapper;
        this.currentUser = currentUser;
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
        EnsureCanRead(enrollment);
        return mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<EnrollmentDto> EnrollAsync(EnrollmentCreateDto dto)
    {
        // Студент может записать только самого себя, иначе роли Student хватило бы для записи любого студента.
        if (currentUser.IsInRole(Roles.Student) && !currentUser.IsInRole(Roles.Admin)
            && currentUser.StudentId != dto.StudentId)
        {
            logger.LogWarning(
                "User {UserName} tried to enroll student {StudentId}",
                currentUser.UserName,
                dto.StudentId);
            throw ApiException.Forbidden("You can only enroll yourself");
        }

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
        EnsureCanGrade(enrollment);

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

    private void EnsureCanRead(Enrollment enrollment)
    {
        if (currentUser.IsInRole(Roles.Admin))
        {
            return;
        }

        if (currentUser.IsInRole(Roles.Teacher) && enrollment.Course?.TeacherId == currentUser.TeacherId)
        {
            return;
        }

        if (currentUser.IsInRole(Roles.Student) && enrollment.StudentId == currentUser.StudentId)
        {
            return;
        }

        logger.LogWarning(
            "User {UserName} tried to read enrollment {EnrollmentId} of another user",
            currentUser.UserName,
            enrollment.Id);
        throw ApiException.Forbidden("You can only access your own enrollments");
    }

    // Преподаватель выставляет оценки только по своим курсам, проверка роли этого не гарантирует.
    private void EnsureCanGrade(Enrollment enrollment)
    {
        if (currentUser.IsInRole(Roles.Admin))
        {
            return;
        }

        if (currentUser.IsInRole(Roles.Teacher) && enrollment.Course?.TeacherId == currentUser.TeacherId)
        {
            return;
        }

        logger.LogWarning(
            "User {UserName} tried to grade enrollment {EnrollmentId} of course {CourseId}",
            currentUser.UserName,
            enrollment.Id,
            enrollment.CourseId);
        throw ApiException.Forbidden("You can only grade students of your own courses");
    }

    private ApiException NotFound(int id)
    {
        logger.LogWarning("Enrollment {EnrollmentId} not found", id);
        return ApiException.NotFound(ErrorCodes.EnrollmentNotFound, "Enrollment not found");
    }
}
