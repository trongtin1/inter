using AutoMapper;
using test.Models.Entity;
using test.Models;
namespace test.Profiles
{
    public class MailProfile : Profile
    {
        public MailProfile()
        {
            CreateMap<Mail, Mail>();
                // .ForMember(dest => dest.FromDate, opt => opt.Condition((src, dest, srcValue) => 
                //     srcValue != null && 
                //     (DateTime)srcValue >= new DateTime(1753, 1, 1) && 
                //     (DateTime)srcValue <= new DateTime(9999, 12, 31)))
                // .ForMember(dest => dest.ToDate, opt => opt.Condition((src, dest, srcValue) => 
                //     srcValue != null && 
                //     (DateTime)srcValue >= new DateTime(1753, 1, 1) && 
                //     (DateTime)srcValue <= new DateTime(9999, 12, 31)));
        }
    }
} 