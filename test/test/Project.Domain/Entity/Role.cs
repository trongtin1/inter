using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test.Project.Domain.Entity;

public class Role
{
    public int Id { get; set; }
    [Required(ErrorMessage = "The Name field is required.")]
    public string? Name { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RoleModule> RoleModules { get; set; } = new List<RoleModule>();
}