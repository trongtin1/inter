using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using test.Project.API.Attributes;
using test.Project.Application.ServiceInterfaces;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Response.Mails;
using test.Project.Domain.Entity;
namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MailsController : ControllerBase
    {
        private readonly IMailService _mailService;
        

        public MailsController(
            IMailService mailService)
        {
            _mailService = mailService;
          
        }

        [HttpGet]
        [ModulePermission("Mails", requireRead: true)]
        public async Task<ApiResponse<MailRes>> GetMails(
            int? page,
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
           
            int pageSize = 6;
            int pageIndex = page ?? 1;

            return await _mailService.GetPageMail(
                pageIndex, pageSize, id, email, isSend, timeType,
                sendStatus, emailCc, emailBcc, fromDate, toDate);
        }

        [HttpGet("{id}")]
        [ModulePermission("Mails", requireRead: true)]
        public async Task<ApiResponse<Mail>> GetMail(long id)
        {
           
            return await _mailService.GetMailByIdAsync(id);
        }

        [HttpPost]
        [ModulePermission("Mails", requireCreate: true)]
        public async Task<ApiResponse<Mail>> CreateMail(Mail mail)
        {
            return await _mailService.CreateMailAsync(mail);
        }

        [HttpPut("{id}")]
        [ModulePermission("Mails", requireUpdate: true)]
        public async Task<ApiResponse<Mail>> UpdateMail(long id, Mail mail)
        {
           
            return await _mailService.UpdateMailAsync(id, mail);
        }

        [HttpDelete("{id}")]
        [ModulePermission("Mails", requireDelete: true)]
        public async Task<ApiResponse<object>> DeleteMail(long id)
        {
            return await _mailService.DeleteMailAsync(id);
        }

        [HttpPost("deleteMultiple")]
        [ModulePermission("Mails", requireDelete: true)]
        public async Task<ApiResponse<object>> DeleteMultiple([FromBody] List<long> ids)
        {
            return await _mailService.DeleteMultipleMailsAsync(ids);
        }

        [HttpGet("filter-options")]
        public async Task<ApiResponse<object>> GetFilterOptions()
        {
            
            return await _mailService.GetFilterOptionsAsync();
        }

        [HttpGet("getOne")]
        [ModulePermission("Mails", requireRead: true)]
        public async Task<ApiResponse<Mail>> GetOne(
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
            
            return await _mailService.GetOneMail(
                id, email, isSend, timeType, sendStatus,
                emailCc, emailBcc, fromDate, toDate);
        }

        [HttpGet("getList")]
        [ModulePermission("Mails", requireRead: true)]
        public async Task<ApiResponse<List<Mail>>> GetList(
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
            
            return await _mailService.GetListMail(
                id, email, isSend, timeType, sendStatus,
                emailCc, emailBcc, fromDate, toDate);
        }
    }
}
