using AutoMapper;
using test.Project.Application.DTOs.Request.Role;
using test.Project.Application.DTOs.Response.Role;
using test.Project.Domain.Entity;

namespace test.Project.Application.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleRes>();

            CreateMap<RoleReq, Role>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Bỏ qua nếu giá trị là null
        }
    }
}
