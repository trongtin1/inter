using test.Project.Application.DTOs.Response.UserOnline;
using test.Project.Domain.Entity;

namespace test.Project.Application.ServiceInterfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        (string name, string email) GetUserInfoFromToken(string token);
        (UserAccessInfo userInfo, bool isValid) ValidateAndGetUserAccess(string token);
    }
} 