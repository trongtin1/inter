using test.Project.Application.DTOs.Response.ModulePermission;
namespace test.Project.Application.DTOs.Response.RoleModule
{
    public class RoleModuleRes
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<ModulePermissionDTO> Modules { get; set; }
    }
}