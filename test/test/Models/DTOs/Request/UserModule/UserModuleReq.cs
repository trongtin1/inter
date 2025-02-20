namespace test.Models.DTOs.Request.UserModule
{
    public class UserModuleReq
    {
        public int UserId { get; set; }
        public int ModuleId { get; set; }
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    } 
}