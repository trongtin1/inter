using Microsoft.AspNetCore.Mvc;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.PersonalProfile;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalProfileController : ControllerBase
    {
        private readonly IPersonalProfileService _personalProfileService;

        public PersonalProfileController(IPersonalProfileService personalProfileService)
        {
            _personalProfileService = personalProfileService;
        }

        [HttpGet("filter-options")]
        public async Task<ApiResponse<IEnumerable<PersonalProfileDTO>>> GetActiveProfiles()
        {
            return await _personalProfileService.GetActiveProfilesAsync();
        }
    }
}