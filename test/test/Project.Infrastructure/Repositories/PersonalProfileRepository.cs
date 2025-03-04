using Microsoft.EntityFrameworkCore;
using System.Data;
using test.Project.Domain.RepoInterfaces;
using test.Project.Infrastructure.Data;
using test.Project.Application.DTOs.Response.PersonalProfile;

namespace test.Project.Infrastructure.Repositories
{
    public class PersonalProfileRepository : IPersonalProfileRepository
    {
        private readonly ApplicationDbContext _secondDbContext;

        public PersonalProfileRepository(Func<string, ApplicationDbContext> dbContextFactory)
        {
            _secondDbContext = dbContextFactory("WorkFlow193Connection");
        }

        public async Task<IEnumerable<PersonalProfileDTO>> GetActiveProfilesAsync()
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

                return result;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }
    }
} 