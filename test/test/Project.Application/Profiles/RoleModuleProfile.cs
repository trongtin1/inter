using AutoMapper;
using test.Project.Application.DTOs.Request.RoleModule;
using test.Project.Application.DTOs.Response.RoleModule;
using test.Project.Domain.Entity;

namespace test.Project.Application.Profiles
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
