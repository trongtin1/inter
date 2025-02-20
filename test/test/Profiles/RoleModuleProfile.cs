using AutoMapper;
using test.Models.DTOs.Response.RoleModule;
using test.Models.Entity;
using test.Models.DTOs.Request.RoleModule;

namespace test.Profiles
{
    public class RoleModuleProfile : Profile
    {
        public RoleModuleProfile()
        {
            CreateMap<RoleModule, RoleModuleDTO>();

            CreateMap<RoleModuleReq, RoleModule>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Bỏ qua nếu giá trị là null
        }
    }
}
