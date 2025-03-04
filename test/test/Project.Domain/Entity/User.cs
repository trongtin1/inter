using System;

namespace test.Project.Domain.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<UserModule> UserModules { get; set; } = new List<UserModule>();
    }

}