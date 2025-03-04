using AutoMapper;
using test.Project.Domain.Entity;
namespace test.Project.Application.Profiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, Notification>();
        }
    }
} 