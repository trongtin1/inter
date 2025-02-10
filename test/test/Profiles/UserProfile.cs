using AutoMapper;
using test.Models;
using test.Models.Entity;
using test.Models.DTOs.Request.User;
using test.Models.DTOs.Response.User;

namespace test.Profiles
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
