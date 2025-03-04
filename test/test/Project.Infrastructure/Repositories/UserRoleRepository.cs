using Microsoft.EntityFrameworkCore;
using test.Project.Domain.Entity;
using test.Project.Domain.RepoInterfaces;
using test.Project.Infrastructure.Data;
using test.Project.Application.DTOs.Response.UserRoles;
using test.Project.Application.DTOs.Response.Role;

namespace test.Project.Infrastructure.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserRoleRes>> GetAllUserRolesAsync()
        {
            return await _context.UserRole
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .GroupBy(ur => new { ur.UserId, ur.User.Username })
                .Select(g => new UserRoleRes
                {
                    UserId = g.Key.UserId,
                    Username = g.Key.Username,
                    Roles = g.Select(ur => new RoleRes
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name
                    }).ToList()
                })
                .ToListAsync();

        }

        public async Task<UserRoleRes> GetUserRolesByUserIdAsync(int userId)
        {
            return await _context.UserRole
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .GroupBy(ur => new { ur.UserId, ur.User.Username })
                .Select(g => new UserRoleRes
                {
                    UserId = g.Key.UserId,
                    Username = g.Key.Username,
                    Roles = g.Select(ur => new RoleRes
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<int>> GetExistingRoleIdsAsync(int userId, List<int> roleIds)
        {
            return await _context.UserRole
                .Where(ur => ur.UserId == userId && roleIds.Contains(ur.RoleId))
                .Select(ur => ur.RoleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserRole>> CreateUserRolesAsync(int userId, List<int> roleIds)
        {
            var userRoles = roleIds.Select(roleId => new UserRole
            {
                UserId = userId,
                RoleId = roleId
            });

            await _context.UserRole.AddRangeAsync(userRoles);
            return userRoles;
        }

        public async Task<List<UserRole>> GetUserRolesToDeleteAsync(int userId, List<int> roleIds)
        {
            return await _context.UserRole
                .Where(ur => ur.UserId == userId && roleIds.Contains(ur.RoleId))
                .ToListAsync();
        }

        public async Task DeleteUserRolesAsync(IEnumerable<UserRole> userRoles)
        {
            _context.UserRole.RemoveRange(userRoles);
        }
    }
} 