namespace test.Models.DTOs.Request.UserRole
{
    public class UserRoleReq
    {
        public int UserId { get; set; }
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}