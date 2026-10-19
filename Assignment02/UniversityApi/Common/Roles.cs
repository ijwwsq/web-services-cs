namespace UniversityApi.Common;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Teacher = "Teacher";
    public const string Student = "Student";

    public static readonly string[] All = { Admin, Teacher, Student };
}

public static class CustomClaims
{
    public const string TeacherId = "teacherId";
    public const string StudentId = "studentId";
}
