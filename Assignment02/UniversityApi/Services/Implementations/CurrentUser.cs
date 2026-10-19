using System.Security.Claims;
using UniversityApi.Common;
using UniversityApi.Services.Interfaces;

namespace UniversityApi.Services.Implementations;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor accessor;

    public CurrentUser(IHttpContextAccessor accessor)
    {
        this.accessor = accessor;
    }

    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public string? UserId => Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? UserName => Principal?.FindFirstValue(ClaimTypes.Name);

    public int? TeacherId => ParseInt(Principal?.FindFirstValue(CustomClaims.TeacherId));

    public int? StudentId => ParseInt(Principal?.FindFirstValue(CustomClaims.StudentId));

    public bool IsInRole(string role) => Principal?.IsInRole(role) == true;

    private static int? ParseInt(string? value) =>
        int.TryParse(value, out var parsed) ? parsed : null;
}
