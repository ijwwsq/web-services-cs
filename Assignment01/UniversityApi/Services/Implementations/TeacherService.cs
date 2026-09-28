using AutoMapper;
using UniversityApi.Common;
using UniversityApi.Dtos.Teachers;
using UniversityApi.Models;
using UniversityApi.Repositories.Interfaces;
using UniversityApi.Services.Interfaces;

namespace UniversityApi.Services.Implementations;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository teachers;
    private readonly IMapper mapper;
    private readonly ILogger<TeacherService> logger;

    public TeacherService(ITeacherRepository teachers, IMapper mapper, ILogger<TeacherService> logger)
    {
        this.teachers = teachers;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<IReadOnlyList<TeacherDto>> GetAllAsync()
    {
        var entities = await teachers.GetAllAsync();
        return mapper.Map<IReadOnlyList<TeacherDto>>(entities);
    }

    public async Task<TeacherDto> GetByIdAsync(int id)
    {
        var teacher = await teachers.GetByIdAsync(id) ?? throw NotFound(id);
        return mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto> CreateAsync(TeacherCreateDto dto)
    {
        if (await teachers.EmailExistsAsync(dto.Email))
        {
            logger.LogWarning("Teacher with email {Email} already exists", dto.Email);
            throw ApiException.Conflict(ErrorCodes.EmailAlreadyExists, "Teacher with this email already exists");
        }

        var teacher = mapper.Map<Teacher>(dto);
        await teachers.AddAsync(teacher);

        logger.LogInformation("Teacher {TeacherId} created", teacher.Id);
        return mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto> UpdateAsync(int id, TeacherUpdateDto dto)
    {
        var teacher = await teachers.GetByIdAsync(id) ?? throw NotFound(id);

        if (await teachers.EmailExistsAsync(dto.Email, id))
        {
            logger.LogWarning("Email {Email} is already used by another teacher", dto.Email);
            throw ApiException.Conflict(ErrorCodes.EmailAlreadyExists, "Teacher with this email already exists");
        }

        mapper.Map(dto, teacher);
        await teachers.UpdateAsync(teacher);

        logger.LogInformation("Teacher {TeacherId} updated", id);
        return mapper.Map<TeacherDto>(teacher);
    }

    public async Task DeleteAsync(int id)
    {
        var teacher = await teachers.GetByIdAsync(id) ?? throw NotFound(id);
        await teachers.DeleteAsync(teacher);
        logger.LogInformation("Teacher {TeacherId} deleted", id);
    }

    private ApiException NotFound(int id)
    {
        logger.LogWarning("Teacher {TeacherId} not found", id);
        return ApiException.NotFound(ErrorCodes.TeacherNotFound, "Teacher not found");
    }
}
