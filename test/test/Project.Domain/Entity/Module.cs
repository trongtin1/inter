namespace test.Project.Domain.Entity
{
    public class Module
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public bool IsActive { get; set; }
        public ICollection<RoleModule> RoleModules { get; set; } = new List<RoleModule>();
        public ICollection<UserModule> UserModules { get; set; } = new List<UserModule>();
    }
} 