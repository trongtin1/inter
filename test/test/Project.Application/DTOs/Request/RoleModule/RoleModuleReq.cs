namespace test.Project.Application.DTOs.Request.RoleModule
{
    public class RoleModuleReq
    {
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    } 
}   
