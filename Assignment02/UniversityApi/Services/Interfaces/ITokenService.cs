using UniversityApi.Models;

namespace UniversityApi.Services.Interfaces;

public interface ITokenService
{
    (string AccessToken, DateTime ExpiresAt) CreateAccessToken(ApplicationUser user, IList<string> roles);
}
