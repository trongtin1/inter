namespace test.Models.DTOs.Response
{
    public class RoleDTO
    {
        public int Id { get; set; } 
        public string? Name { get; set; }
        public List<UserDTO>? Users { get; set; }
    }
}