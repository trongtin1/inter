namespace test.Project.Application.DTOs.Response.ModulePermission
{
    public class ModulePermissionDTO
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}