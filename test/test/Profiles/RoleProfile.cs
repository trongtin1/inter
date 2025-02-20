using AutoMapper;
using test.Models.Entity;
using test.Models.DTOs.Response.Role;
using test.Models.DTOs.Request.Role;

namespace test.Profiles
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
