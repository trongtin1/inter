using test.Project.Domain.RepoInterfaces;

namespace test.Project.Application.ServiceInterfaces;

public interface IUnitOfWork : IDisposable
{
    IMailRepository Mails { get; }
    IMailExcelRepository MailExcel { get; }
    IMailStatisticsRepository MailStatistics { get; }
    INotificationRepository Notifications { get; }
    IAuthRepository AuthRepository { get; }
    IModuleRepository Modules { get; }
    INotiStatisticsRepository NotiStatistics { get; }
    IRoleModuleRepository RoleModules { get; }
    IRoleRepository Roles { get; }
    IUserModuleRepository UserModules { get; }
    IUserRoleRepository UserRoles { get; }
    IUserOnlineRepository UserOnline { get; }
    IUserRepository Users { get; }
    IPersonalProfileRepository PersonalProfiles { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
} 

