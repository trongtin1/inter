using AutoMapper;
using test.Project.Application.DTOs.Request.Module;
using test.Project.Application.DTOs.Response.Module;
using test.Project.Domain.Entity;

namespace test.Project.Application.Profiles
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<Module, ModuleRes>();

            CreateMap<ModuleReq, Module>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Bỏ qua nếu giá trị là null
        }
    }
}
