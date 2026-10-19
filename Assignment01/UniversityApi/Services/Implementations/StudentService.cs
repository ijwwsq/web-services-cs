using AutoMapper;
using UniversityApi.Common;
using UniversityApi.Dtos.Students;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;
using UniversityApi.Services.Interfaces;

namespace UniversityApi.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IStudentRepository students;
    private readonly IMapper mapper;
    private readonly ICurrentUser currentUser;
    private readonly ILogger<StudentService> logger;

    public StudentService(
        IStudentRepository students,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<StudentService> logger)
    {
        this.students = students;
        this.mapper = mapper;
        this.currentUser = currentUser;
        this.logger = logger;
    }

    public async Task<IReadOnlyList<StudentDto>> GetAllAsync()
    {
        var entities = await students.GetAllAsync();
        return mapper.Map<IReadOnlyList<StudentDto>>(entities);
    }

    public async Task<StudentDto> GetByIdAsync(int id)
    {
        EnsureOwnStudentData(id);
        var student = await students.GetByIdAsync(id) ?? throw NotFound(id);
        return mapper.Map<StudentDto>(student);
    }

    public async Task<StudentWithCoursesDto> GetWithCoursesAsync(int id)
    {
        EnsureOwnStudentData(id);
        var student = await students.GetWithCoursesAsync(id) ?? throw NotFound(id);
        return mapper.Map<StudentWithCoursesDto>(student);
    }

    public async Task<StudentDto> CreateAsync(StudentCreateDto dto)
    {
        if (await students.EmailExistsAsync(dto.Email))
        {
            logger.LogWarning("Student with email {Email} already exists", dto.Email);
            throw ApiException.Conflict(ErrorCodes.EmailAlreadyExists, "Student with this email already exists");
        }

        var student = mapper.Map<Student>(dto);
        student.CreatedAt = DateTime.UtcNow;
        await students.AddAsync(student);

        logger.LogInformation("Student {StudentId} created", student.Id);
        return mapper.Map<StudentDto>(student);
    }

    public async Task<StudentDto> UpdateAsync(int id, StudentUpdateDto dto)
    {
        var student = await students.GetByIdAsync(id) ?? throw NotFound(id);

        if (await students.EmailExistsAsync(dto.Email, id))
        {
            logger.LogWarning("Email {Email} is already used by another student", dto.Email);
            throw ApiException.Conflict(ErrorCodes.EmailAlreadyExists, "Student with this email already exists");
        }

        mapper.Map(dto, student);
        await students.UpdateAsync(student);

        logger.LogInformation("Student {StudentId} updated", id);
        return mapper.Map<StudentDto>(student);
    }

    public async Task DeleteAsync(int id)
    {
        var student = await students.GetByIdAsync(id) ?? throw NotFound(id);
        await students.DeleteAsync(student);
        logger.LogInformation("Student {StudentId} deleted", id);
    }

    // Роли мало: студент видит только собственные данные, иначе чужие оценки были бы доступны любому студенту.
    private void EnsureOwnStudentData(int studentId)
    {
        if (!currentUser.IsInRole(Roles.Student) || currentUser.IsInRole(Roles.Admin))
        {
            return;
        }

        if (currentUser.StudentId != studentId)
        {
            logger.LogWarning(
                "User {UserName} tried to read data of student {StudentId}",
                currentUser.UserName,
                studentId);
            throw ApiException.Forbidden("You can only access your own student data");
        }
    }

    private ApiException NotFound(int id)
    {
        logger.LogWarning("Student {StudentId} not found", id);
        return ApiException.NotFound(ErrorCodes.StudentNotFound, "Student not found");
    }
}
