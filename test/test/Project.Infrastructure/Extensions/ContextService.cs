using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.Infrastructure.Extensions
{

    public interface IContextService
    {
        string GetUserId();
        string GetUserEmail();
        List<string> GetUserRoles();
        (string id, string userEmail, List<string> rolesList) GetUserClaims();
    }

    public class ContextService : IContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "id")?.Value ?? "";
        }

        public string GetUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "email")?.Value ?? "";
        }

        public List<string> GetUserRoles()
        {
            var roles = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "roles")?.Value ?? "";
            return roles.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public (string id, string userEmail, List<string> rolesList) GetUserClaims()
        {
            return (GetUserId(), GetUserEmail(), GetUserRoles());
        }
    }
} 