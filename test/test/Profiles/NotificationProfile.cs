using AutoMapper;
using test.Models.Entity;
using test.Models;
namespace test.Profiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, Notification>();
        }
    }
} 