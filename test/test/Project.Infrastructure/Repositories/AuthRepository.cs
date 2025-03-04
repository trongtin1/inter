using Microsoft.EntityFrameworkCore;
using test.Project.Domain.RepoInterfaces;
using test.Project.Domain.Entity;
using test.Project.Infrastructure.Data;

namespace test.Project.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.User
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.User
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.User.FindAsync(id);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.User.Update(user);
            //await _context.SaveChangesAsync();
        }
    }
} 