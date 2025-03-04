using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.PersonalProfile;

namespace test.Project.Application.ServiceInterfaces
{
    public interface IPersonalProfileService
    {
        Task<ApiResponse<IEnumerable<PersonalProfileDTO>>> GetActiveProfilesAsync();
    }
} 