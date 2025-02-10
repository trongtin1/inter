using AutoMapper;
using test.Models.DTOs.Response.Module;
using test.Models.Entity;
using test.Models.DTOs.Request.Module;

namespace test.Profiles
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
