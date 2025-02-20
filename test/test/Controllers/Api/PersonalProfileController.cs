using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using test.Services;
using test.Models.DTOs.Response.PersonalProfile;
using test.Models;
using test.Models.DTOs;

namespace test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _secondDbContext;

        public PersonalProfileController(Func<string, ApplicationDbContext> dbContextFactory)
        {
            // Sử dụng WorkFlow193Connection
            _secondDbContext = dbContextFactory("WorkFlow193Connection");
        }

        [HttpGet("filter-options")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersonalProfileDTO>>>> GetActiveProfiles()
        {
            var result = new List<PersonalProfileDTO>();
            
            var connection = _secondDbContext.Database.GetDbConnection();
            try
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = @"SELECT Id, FullName, Email 
                                      FROM PersonalProfile 
                                      WHERE EnableEmail = 1 AND IsDeleted = 0 AND UserStatus <> -1";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new PersonalProfileDTO
                    {
                        Id = reader.GetInt64(0),
                        FullName = reader.GetString(1),
                        Email = reader.GetString(2)
                    });
                }

                var response = new ApiResponse<IEnumerable<PersonalProfileDTO>>
                {
                    Success = true,
                    Message = "Success",
                    Data = result,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }
    }
}