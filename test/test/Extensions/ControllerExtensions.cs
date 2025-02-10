using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace test.Extensions
{
    public static class ControllerExtensions
    {
        public static (string id,string userEmail, List<string> rolesList) GetUserClaims(this ControllerBase controller)
        {
            var id = controller.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value ?? "";
            var roles = controller.User.Claims.FirstOrDefault(c => c.Type == "roles")?.Value ?? "";
            var userEmail = controller.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var rolesList = roles.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            return (id,userEmail, rolesList);
        }
    }
} 