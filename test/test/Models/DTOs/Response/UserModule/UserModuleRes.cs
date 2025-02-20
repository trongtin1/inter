using test.Models.DTOs.Response.ModulePermission;

namespace test.Models.DTOs.Response.UserModule
{
    public class UserModuleRes
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<ModulePermissionDTO> Modules { get; set; }
    }
}