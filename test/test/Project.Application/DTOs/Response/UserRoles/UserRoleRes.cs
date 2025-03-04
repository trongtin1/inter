using test.Project.Application.DTOs.Response.Role;

namespace test.Project.Application.DTOs.Response.UserRoles
{
    public class UserRoleRes
    {   
        public int UserId { get; set; }
        public string? Username { get; set; }
        public List<RoleRes> Roles { get; set; }
    }



}