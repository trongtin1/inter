namespace test.Models.DTOs.Request.UserRole
{
    public class DeleteUserRolesReq
    {
        public int UserId { get; set; }
        public List<int> RoleIds { get; set; }
    }
} 