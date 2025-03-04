namespace test.Project.Domain.Entity
{
    public class RoleModule
    {
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public int ModuleId { get; set; }
        public Module? Module { get; set; }
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    } 
}
