using AutoMapper;
using test.Project.Application.DTOs.Request.UserModule;
using test.Project.Application.DTOs.Response.UserModule;
using test.Project.Domain.Entity;

namespace test.Project.Application.Profiles
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
