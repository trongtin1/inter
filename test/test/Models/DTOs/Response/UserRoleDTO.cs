namespace test.Models.DTOs.Response
{
    public class UserRoleDTO
    {   
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Username { get; set; }
    }
}