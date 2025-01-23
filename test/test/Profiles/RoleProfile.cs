using AutoMapper;
using test.Models;
using test.Models.DTOs.Response;
using test.Models.DTOs.Request;
namespace test.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, Role>();
            CreateMap<Role, RoleDTO>();
            CreateMap<UpdateRoleDTO, Role>();
        }
    }
}