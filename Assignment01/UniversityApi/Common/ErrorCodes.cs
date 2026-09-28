namespace UniversityApi.Common;

public static class ErrorCodes
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string StudentNotFound = "STUDENT_NOT_FOUND";
    public const string TeacherNotFound = "TEACHER_NOT_FOUND";
    public const string CourseNotFound = "COURSE_NOT_FOUND";
    public const string EnrollmentNotFound = "ENROLLMENT_NOT_FOUND";
    public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";
    public const string DuplicateEnrollment = "DUPLICATE_ENROLLMENT";
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
}
