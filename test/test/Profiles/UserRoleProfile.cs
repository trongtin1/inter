using AutoMapper;
using test.Models.DTOs.Response.UserRole;
using test.Models.Entity;
using test.Models.DTOs.Request.UserRole;


namespace test.Profiles
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
