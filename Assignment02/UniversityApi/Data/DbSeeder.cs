using Microsoft.EntityFrameworkCore;
using UniversityApi.Models;

namespace UniversityApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Teachers.AnyAsync())
        {
            return;
        }

        var teachers = new List<Teacher>
        {
            new() { FirstName = "Farida", LastName = "Abdoldina", Email = "f.abdoldina@university.kz", Department = "Software Engineering" },
            new() { FirstName = "Evgeniy", LastName = "Gertsen", Email = "e.gertsen@university.kz", Department = "Software Engineering" },
            new() { FirstName = "Aigul", LastName = "Nurlanova", Email = "a.nurlanova@university.kz", Department = "Information Systems" }
        };
        context.Teachers.AddRange(teachers);
        await context.SaveChangesAsync();

        var students = new List<Student>
        {
            new() { FirstName = "Alex", LastName = "Smith", Email = "alex.smith@university.kz", BirthDate = new DateOnly(2004, 3, 12) },
            new() { FirstName = "Anna", LastName = "Ivanova", Email = "anna.ivanova@university.kz", BirthDate = new DateOnly(2003, 11, 2) },
            new() { FirstName = "Max", LastName = "Petrov", Email = "max.petrov@university.kz", BirthDate = new DateOnly(2004, 7, 25) },
            new() { FirstName = "Dana", LastName = "Kim", Email = "dana.kim@university.kz", BirthDate = new DateOnly(2005, 1, 9) }
        };
        context.Students.AddRange(students);
        await context.SaveChangesAsync();

        var courses = new List<Course>
        {
            new() { Name = "Web Services", Description = "REST API on ASP.NET Core", Credits = 5, TeacherId = teachers[1].Id },
            new() { Name = "Databases", Description = "Relational databases and SQL", Credits = 4, TeacherId = teachers[0].Id },
            new() { Name = "Algorithms", Description = "Data structures and algorithms", Credits = 6, TeacherId = teachers[0].Id },
            new() { Name = "Information Systems", Description = "Design of information systems", Credits = 3, TeacherId = teachers[2].Id }
        };
        context.Courses.AddRange(courses);
        await context.SaveChangesAsync();

        var enrollments = new List<Enrollment>
        {
            new() { StudentId = students[0].Id, CourseId = courses[0].Id, Grade = 90 },
            new() { StudentId = students[0].Id, CourseId = courses[1].Id, Grade = 75 },
            new() { StudentId = students[1].Id, CourseId = courses[0].Id },
            new() { StudentId = students[2].Id, CourseId = courses[2].Id, Grade = 82 }
        };
        context.Enrollments.AddRange(enrollments);
        await context.SaveChangesAsync();

        logger.LogInformation(
            "Database seeded: {Teachers} teachers, {Students} students, {Courses} courses, {Enrollments} enrollments",
            teachers.Count,
            students.Count,
            courses.Count,
            enrollments.Count);
    }
}
