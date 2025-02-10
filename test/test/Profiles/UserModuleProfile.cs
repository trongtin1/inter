using AutoMapper;
using test.Models.DTOs.Response.UserModule;
using test.Models.Entity;
using test.Models.DTOs.Request.UserModule;

namespace test.Profiles
{
    public class UserModuleProfile : Profile
    {
        public UserModuleProfile()
        {
            CreateMap<UserModule, UserModuleDTO>();

            CreateMap<UserModuleReq, UserModule>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Bỏ qua nếu giá trị là null
        }
    }
}
