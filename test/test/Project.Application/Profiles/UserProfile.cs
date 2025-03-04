using AutoMapper;
using test.Project.Application.DTOs.Request.User;
using test.Project.Application.DTOs.Response.User;
using test.Project.Domain.Entity;

namespace test.Project.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserRes>();

            CreateMap<UserReq, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); 
        }
    }
}
