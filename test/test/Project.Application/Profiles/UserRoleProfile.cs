using AutoMapper;
using test.Project.Application.DTOs.Request.UserRole;
using test.Project.Application.DTOs.Response.UserRoles;
using test.Project.Domain.Entity;


namespace test.Project.Application.Profiles
{
    public class UserRoleProfile : Profile
    {
        public UserRoleProfile()
        {

            CreateMap<UserRole, UserRoleRes>();
            CreateMap<UserRoleReq, UserRole>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Bỏ qua nếu giá trị là null
                
        }
    }
}
