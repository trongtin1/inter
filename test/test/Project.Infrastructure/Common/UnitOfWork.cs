using Microsoft.EntityFrameworkCore.Storage;
using test.Project.Application.ServiceInterfaces;
using test.Project.Domain.RepoInterfaces;
using test.Project.Infrastructure.Data;
using Microsoft.Extensions.Caching.Memory;
using test.Project.Infrastructure.Repositories;

namespace test.Project.Infrastructure.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
        private IMailRepository _mails;
        private IMailExcelRepository _mailExcel;
        private IMailStatisticsRepository _mailStatistics;
        private INotificationRepository _notifications;
        private IAuthRepository _authRepository;
        private IModuleRepository _modules;
        private INotiStatisticsRepository _notiStatistics;
        private IRoleModuleRepository _roleModules;
        private IRoleRepository _roles;
        private IUserModuleRepository _userModules;
        private IUserRoleRepository _userRoles;
        private IUserOnlineRepository _userOnline;
        private IUserRepository _users;
        private IPersonalProfileRepository _personalProfiles;
        private bool _disposed;
        private readonly IMemoryCache _memoryCache;
        private readonly Func<string, ApplicationDbContext> _dbContextFactory;

        public UnitOfWork(ApplicationDbContext context, IMemoryCache memoryCache, Func<string, ApplicationDbContext> dbContextFactory)
        {
            _context = context;
            _memoryCache = memoryCache;
            _dbContextFactory = dbContextFactory;
        }

        public IMailRepository Mails => _mails ??= new MailRepository(_context);
        public IMailExcelRepository MailExcel => _mailExcel ??= new MailExcelRepository(_context);
        public IMailStatisticsRepository MailStatistics => _mailStatistics ??= new MailStatisticsRepository(_context);
        public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_context);
        public IAuthRepository AuthRepository => _authRepository ??= new AuthRepository(_context);
        public IModuleRepository Modules => _modules ??= new ModuleRepository(_context);
        public INotiStatisticsRepository NotiStatistics => _notiStatistics ??= new NotiStatisticsRepository(_context);
        public IRoleModuleRepository RoleModules => _roleModules ??= new RoleModuleRepository(_context);
        public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
        public IUserModuleRepository UserModules => _userModules ??= new UserModuleRepository(_context);
        public IUserRoleRepository UserRoles => _userRoles ??= new UserRoleRepository(_context);
        public IUserOnlineRepository UserOnline => _userOnline ??= new UserOnlineRepository(_memoryCache);
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IPersonalProfileRepository PersonalProfiles => 
            _personalProfiles ??= new PersonalProfileRepository(_dbContextFactory);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }
        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
                _transaction?.Dispose();
            }
            _disposed = true;
        }
    }
}
