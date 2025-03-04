using test.Project.Domain.Entity;
using test.Project.Application.DTOs.Response.PersonalProfile;

namespace test.Project.Domain.RepoInterfaces
{
    public interface IPersonalProfileRepository
    {
        Task<IEnumerable<PersonalProfileDTO>> GetActiveProfilesAsync();
    }
} 