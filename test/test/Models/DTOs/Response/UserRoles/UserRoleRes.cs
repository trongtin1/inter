using test.Models.DTOs.Response.Role;

namespace test.Models.DTOs.Response.UserRole
{
    public class UserRoleRes
    {   
        public int UserId { get; set; }
        public string? Username { get; set; }
        public List<RoleRes> Roles { get; set; }
    }



}