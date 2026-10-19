using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityApi.Common;
using UniversityApi.Models;

namespace UniversityApi.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger logger)
    {
        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("Role {Role} created", role);
            }
        }

        var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.Email == "e.gertsen@university.kz");
        var otherTeacher = await context.Teachers.FirstOrDefaultAsync(t => t.Email == "f.abdoldina@university.kz");
        var student = await context.Students.FirstOrDefaultAsync(s => s.Email == "alex.smith@university.kz");
        var otherStudent = await context.Students.FirstOrDefaultAsync(s => s.Email == "anna.ivanova@university.kz");

        var section = configuration.GetSection("SeedUsers");

        await EnsureUser(userManager, logger, "admin", "admin@university.kz",
            section["AdminPassword"] ?? "Admin123$", Roles.Admin, null, null);

        await EnsureUser(userManager, logger, "teacher1", "teacher1@university.kz",
            section["TeacherPassword"] ?? "Teacher123$", Roles.Teacher, teacher?.Id, null);

        await EnsureUser(userManager, logger, "teacher2", "teacher2@university.kz",
            section["TeacherPassword"] ?? "Teacher123$", Roles.Teacher, otherTeacher?.Id, null);

        await EnsureUser(userManager, logger, "student1", "student1@university.kz",
            section["StudentPassword"] ?? "Student123$", Roles.Student, null, student?.Id);

        await EnsureUser(userManager, logger, "student2", "student2@university.kz",
            section["StudentPassword"] ?? "Student123$", Roles.Student, null, otherStudent?.Id);
    }

    private static async Task EnsureUser(
        UserManager<ApplicationUser> userManager,
        ILogger logger,
        string userName,
        string email,
        string password,
        string role,
        int? teacherId,
        int? studentId)
    {
        if (await userManager.FindByNameAsync(userName) != null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true,
            TeacherId = teacherId,
            StudentId = studentId
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            logger.LogError(
                "Seed user {UserName} was not created: {Errors}",
                userName,
                string.Join("; ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(user, role);
        logger.LogInformation("Seed user {UserName} created with role {Role}", userName, role);
    }
}
