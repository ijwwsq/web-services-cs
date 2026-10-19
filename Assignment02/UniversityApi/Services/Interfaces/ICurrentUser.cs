namespace UniversityApi.Services.Interfaces;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    string? UserId { get; }
    string? UserName { get; }
    int? TeacherId { get; }
    int? StudentId { get; }
    bool IsInRole(string role);
}
