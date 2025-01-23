using AutoMapper;
using test.Models;
using test.Models.DTOs.Response;
using test.Models.DTOs.Request;
namespace test.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, User>();
            CreateMap<User, UserDTO>();
            CreateMap<UpdateUserDTO, User>();
        }
    }
}