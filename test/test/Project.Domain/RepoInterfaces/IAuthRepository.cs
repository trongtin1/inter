using test.Project.Domain.Entity;

namespace test.Project.Domain.RepoInterfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);
    }
} 