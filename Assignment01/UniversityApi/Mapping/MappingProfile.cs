using AutoMapper;
using UniversityApi.Dtos.Courses;
using UniversityApi.Dtos.Enrollments;
using UniversityApi.Dtos.Students;
using UniversityApi.Dtos.Teachers;
using UniversityApi.Models;

namespace UniversityApi.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Student, StudentDto>();
        CreateMap<StudentCreateDto, Student>();
        CreateMap<StudentUpdateDto, Student>();

        CreateMap<Student, StudentWithCoursesDto>()
            .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.Enrollments));

        CreateMap<Enrollment, StudentCourseDto>()
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Course!.Name))
            .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.Course!.Credits))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src =>
                src.Course!.Teacher!.FirstName + " " + src.Course.Teacher.LastName));

        CreateMap<Teacher, TeacherDto>();
        CreateMap<TeacherCreateDto, Teacher>();
        CreateMap<TeacherUpdateDto, Teacher>();

        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src =>
                src.Teacher!.FirstName + " " + src.Teacher.LastName));
        CreateMap<CourseCreateDto, Course>();
        CreateMap<CourseUpdateDto, Course>();

        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src =>
                src.Student!.FirstName + " " + src.Student.LastName))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course!.Name));
    }
}
