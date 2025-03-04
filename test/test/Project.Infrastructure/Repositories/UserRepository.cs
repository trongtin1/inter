using Microsoft.EntityFrameworkCore;
using test.Project.Domain.Entity;
using test.Project.Domain.RepoInterfaces;
using test.Project.Infrastructure.Data;
using test.Project.Application.DTOs.Response.User;
using test.Project.Application.DTOs.Request.User;
using test.Project.Infrastructure.Common;
using System.Linq.Expressions;

namespace test.Project.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<bool> IsUsernameExistsAsync(string username, int? excludeId = null)
        {
            return await _dbSet
                .AnyAsync(u => u.Username == username && (!excludeId.HasValue || u.Id != excludeId));
        }

        public override async Task<User> UpdateAsync(User entity)
        {
            var existingUser = await _dbSet.FindAsync(entity.Id);
            if (existingUser != null)
            {
                // Cập nhật các trường cơ bản
                existingUser.Username = entity.Username;
                existingUser.Email = entity.Email;
                
                // Chỉ cập nhật password nếu có password mới
                if (!string.IsNullOrEmpty(entity.Password))
                {
                    existingUser.Password = BCrypt.Net.BCrypt.HashPassword(entity.Password);
                }

                _context.Entry(existingUser).State = EntityState.Modified;
            }
            return existingUser;
        }

        public override async Task<User> CreateAsync(User entity)
        {
            // Hash password trước khi tạo mới
            entity.Password = BCrypt.Net.BCrypt.HashPassword(entity.Password);
            await _dbSet.AddAsync(entity);

            return entity;
        }

        public async Task<User> GetOne(string? username, string? email)
        {
            return await GetOneAsync(u => 
                (string.IsNullOrEmpty(username) || u.Username == username) && 
                (string.IsNullOrEmpty(email) || u.Email == email));
        }

        public async Task<List<User>> GetList(string? username, string? email)
        {
            return await GetListAsync(u => 
                (string.IsNullOrEmpty(username) || u.Username == username) && 
                (string.IsNullOrEmpty(email) || u.Email == email));
        }

        public async Task<(IEnumerable<User> Items, int TotalCount)> GetPage(
            string? username,
            string? email,
            int pageIndex,
            int pageSize)
        {
            Expression<Func<User, bool>> predicate = x => 
                (string.IsNullOrEmpty(username) || x.Username.Contains(username)) && 
                (string.IsNullOrEmpty(email) || x.Email.Contains(email));
            
            return await FindPagedListAsync(predicate, pageIndex, pageSize);
        }
    }
} 