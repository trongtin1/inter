using AutoMapper;
using test.Project.Application.ServiceInterfaces;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.Mails;
using test.Project.Domain.Entity;
using test.Project.Infrastructure.Extensions;

namespace test.Project.Application.Services;

public class MailService : IMailService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMailNotificationService _notificationService;
    private readonly IContextService _contextService;

    public MailService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        IMailNotificationService notificationService,
        IContextService contextService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
        _contextService = contextService;
    }

    public async Task<ApiResponse<MailRes>> GetPageMail(
        int pageIndex,
        int pageSize,
        string? id,
        string? email,
        bool? isSend,
        string? timeType,
        string? sendStatus,
        string? emailCc,
        string? emailBcc,
        DateTime? fromDate,
        DateTime? toDate)
    {
        try
        {
            var userEmail = _contextService.GetUserEmail();
            var rolesList = _contextService.GetUserRoles();

            if (!rolesList.Contains("admin"))
            {
                email = userEmail;
            }

            var mails = await _unitOfWork.Mails.GetPage(pageIndex, id, email, isSend, timeType, 
                                                              sendStatus, emailCc, emailBcc, fromDate, toDate);
           
            return new ApiResponse<MailRes>
            {
                Success = true,
                Message = "Successfully retrieved mails.",
                Data = mails
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<MailRes>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<Mail>> GetMailByIdAsync(
        long id)
    {
        try
        {
            var userEmail = _contextService.GetUserEmail();
            var rolesList = _contextService.GetUserRoles();

            var mail = await _unitOfWork.Mails.GetByIdAsync(id);
            
            if (mail == null || !rolesList.Contains("admin") && mail.Email != userEmail)
            {
                return new ApiResponse<Mail>
                {
                    Success = false,
                    Message = "Access denied",
                    Data = null
                };
            }
            
            return new ApiResponse<Mail>
            {
                Success = true,
                Message = "Mail retrieved successfully",
                Data = mail
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<Mail>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<Mail>> CreateMailAsync(Mail mail)
    {
        try
        {   
            await _unitOfWork.BeginTransactionAsync();

            if (!string.IsNullOrEmpty(mail.EmailCc))
            {
                mail.EmailCc = ";" + mail.EmailCc.Replace(",", ";").Trim(';') + ";";
            }
            
            if (!string.IsNullOrEmpty(mail.EmailBcc))
            {
                mail.EmailBcc = ";" + mail.EmailBcc.Replace(",", ";").Trim(';') + ";";
            }

            var createdMail = await _unitOfWork.Mails.CreateAsync(mail);
            //await _unitOfWork.SaveChangesAsync();
            
            await _unitOfWork.CommitAsync();
            await _notificationService.NotifyMailCreated(string.Join(",", mail.Id));
            return new ApiResponse<Mail>
            {
                Success = true,
                Message = "Mail created successfully",
                Data = createdMail
            };
        }
        catch (Exception ex)
        {
            //await _unitOfWork.RollbackAsync();
            return new ApiResponse<Mail>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<Mail>> UpdateMailAsync(
        long id,
        Mail mail)
    {
        try
        {
            var existingMail = await _unitOfWork.Mails.GetByIdAsync(id);
            var userEmail = _contextService.GetUserEmail();
            var rolesList = _contextService.GetUserRoles();

            if (existingMail == null || !rolesList.Contains("admin") && existingMail.Email != userEmail)
            {
                return new ApiResponse<Mail>
                {
                    Success = false,
                    Message = "Access denied",
                    Data = null
                };
            }

            await _unitOfWork.BeginTransactionAsync();

            if (!string.IsNullOrEmpty(mail.EmailCc))
            {
                mail.EmailCc = ";" + mail.EmailCc.Replace(",", ";").Trim(';') + ";";
            }
            
            if (!string.IsNullOrEmpty(mail.EmailBcc))
            {
                mail.EmailBcc = ";" + mail.EmailBcc.Replace(",", ";").Trim(';') + ";";
            }

            _mapper.Map(mail, existingMail);
            var updatedMail = await _unitOfWork.Mails.UpdateAsync(existingMail);
            //await _unitOfWork.SaveChangesAsync();
            
            await _notificationService.NotifyMailUpdated(id.ToString());
            
            await _unitOfWork.CommitAsync();

            return new ApiResponse<Mail>
            {
                Success = true,
                Message = "Mail updated successfully",
                Data = updatedMail
            };
        }
        catch (Exception ex)
        {
            
            return new ApiResponse<Mail>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<object>> DeleteMailAsync(long id)
    {
        try
        {
            var mail = await _unitOfWork.Mails.GetByIdAsync(id);
            if (mail == null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Mail not found"
                };
            }

            await _unitOfWork.BeginTransactionAsync();

            await _unitOfWork.Mails.DeleteAsync(mail);
            //await _unitOfWork.SaveChangesAsync();
            
            await _notificationService.NotifyMailDeleted(id.ToString());

            await _unitOfWork.CommitAsync();

            return new ApiResponse<object>
            {
                Success = true,
                Message = "Mail deleted successfully"
            };
        }
        catch (Exception ex)
        {
            //await _unitOfWork.RollbackAsync();
            return new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<object>> DeleteMultipleMailsAsync(List<long> ids)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var mails = await _unitOfWork.Mails.GetByIdsAsync(ids);
            await _unitOfWork.Mails.DeleteRangeAsync(mails);
            //await _unitOfWork.SaveChangesAsync();
            
            await _notificationService.NotifyMailDeleted(string.Join(",", ids));

            await _unitOfWork.CommitAsync();

            return new ApiResponse<object>
            {
                Success = true,
                Message = "Mails deleted successfully"
            };
        }
        catch (Exception ex)
        {
            //await _unitOfWork.RollbackAsync();
            return new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<object>> GetFilterOptionsAsync()
    {
        try
        {
            var userEmail = _contextService.GetUserEmail();
            var rolesList = _contextService.GetUserRoles();

            var data = await _unitOfWork.Mails.GetFilterDataAsync(userEmail, rolesList);

            var filterOptions = new
            {
                ids = data
                    .Select(m => m.Id.ToString())
                    .Distinct()
                    .ToList(),

                emails = !rolesList.Contains("admin")
                    ? new List<string> { userEmail }
                    : data
                        .Select(m => m.Email)
                        .Where(e => !string.IsNullOrEmpty(e))
                        .Distinct()
                        .ToList(),

                emailCcs = data
                    .Where(m => !string.IsNullOrEmpty(m.EmailCc))
                    .SelectMany(m => m.EmailCc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    .Distinct()
                    .ToList(),

                emailBccs = data
                    .Where(m => !string.IsNullOrEmpty(m.EmailBcc))
                    .SelectMany(m => m.EmailBcc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    .Distinct()
                    .ToList()
            };

            return new ApiResponse<object>
            {
                Success = true,
                Data = filterOptions
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<Mail>> GetOneMail(
        string? id,
        string? email,
        bool? isSend,
        string? timeType,
        string? sendStatus,
        string? emailCc,
        string? emailBcc,
        DateTime? fromDate,
        DateTime? toDate)
    {
        try
        {
            var userEmail = _contextService.GetUserEmail();
            var rolesList = _contextService.GetUserRoles();

            if (!rolesList.Contains("admin"))
            {
                email = userEmail;
            }

            var mail = await _unitOfWork.Mails.GetOne(
                id, email, isSend, timeType, sendStatus, 
                emailCc, emailBcc, fromDate, toDate);
            
            if (mail == null)
            {
                return new ApiResponse<Mail>
                {
                    Success = false,
                    Message = "Mail not found"
                };
            }

            var mailRes = _mapper.Map<Mail>(mail);
            return new ApiResponse<Mail>
            {
                Success = true,
                Message = "Mail retrieved successfully",
                Data = mailRes
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<Mail>
            {
                Success = false,
                Message = "Error retrieving mail: " + ex.Message
            };
        }
    }

    public async Task<ApiResponse<List<Mail>>> GetListMail(
        string? id,
        string? email,
        bool? isSend,
        string? timeType,
        string? sendStatus,
        string? emailCc,
        string? emailBcc,
        DateTime? fromDate,
        DateTime? toDate)
    {
        try
        {
            var userEmail = _contextService.GetUserEmail();
            var rolesList = _contextService.GetUserRoles();

            if (!rolesList.Contains("admin"))
            {
                email = userEmail;
            }

            var mails = await _unitOfWork.Mails.GetList(
                id, email, isSend, timeType, sendStatus, 
                emailCc, emailBcc, fromDate, toDate);
            
            var mailsRes = _mapper.Map<List<Mail>>(mails);

            return new ApiResponse<List<Mail>>
            {
                Success = true,
                Message = "Mails retrieved successfully",
                Data = mailsRes
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<Mail>>
            {
                Success = false,
                Message = "Error retrieving mails: " + ex.Message
            };
        }
    }
}

