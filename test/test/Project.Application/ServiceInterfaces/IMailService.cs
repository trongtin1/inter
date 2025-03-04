using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.Mails;
using test.Project.Domain.Entity;


namespace test.Project.Application.ServiceInterfaces;

public interface IMailService
{
    Task<ApiResponse<MailRes>> GetPageMail(
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
        DateTime? toDate);

    Task<ApiResponse<Mail>> GetMailByIdAsync(
        long id);

    Task<ApiResponse<Mail>> CreateMailAsync(Mail mail);

    Task<ApiResponse<Mail>> UpdateMailAsync(
        long id,
        Mail mail);

    Task<ApiResponse<object>> DeleteMailAsync(long id);

    Task<ApiResponse<object>> DeleteMultipleMailsAsync(List<long> ids);

    Task<ApiResponse<object>> GetFilterOptionsAsync();

    Task<ApiResponse<Mail>> GetOneMail(
        string? id,
        string? email,
        bool? isSend,
        string? timeType,
        string? sendStatus,
        string? emailCc,
        string? emailBcc,
        DateTime? fromDate,
        DateTime? toDate);

    Task<ApiResponse<List<Mail>>> GetListMail(
        string? id,
        string? email,
        bool? isSend,
        string? timeType,
        string? sendStatus,
        string? emailCc,
        string? emailBcc,
        DateTime? fromDate,
        DateTime? toDate);
}