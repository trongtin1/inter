namespace test.Project.Application.DTOs.Response.UserModule
{
    public class UserModuleDTO
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public int ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
} 