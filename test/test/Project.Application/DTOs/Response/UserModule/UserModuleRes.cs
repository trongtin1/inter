using test.Project.Application.DTOs.Response.ModulePermission;

namespace test.Project.Application.DTOs.Response.UserModule
{
    public class UserModuleRes
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<ModulePermissionDTO> Modules { get; set; }
    }
}