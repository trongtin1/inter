using test.Project.Application.ServiceInterfaces;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.PersonalProfile;

namespace test.Project.Application.Services
{
    public class PersonalProfileService : IPersonalProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PersonalProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IEnumerable<PersonalProfileDTO>>> GetActiveProfilesAsync()
        {
            try
            {
                var profiles = await _unitOfWork.PersonalProfiles.GetActiveProfilesAsync();
                return new ApiResponse<IEnumerable<PersonalProfileDTO>>
                {
                    Success = true,
                    Message = "Success",
                    Data = profiles
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<PersonalProfileDTO>>
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                };
            }
        }
    }
} 