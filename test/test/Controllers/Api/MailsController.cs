using Microsoft.AspNetCore.Mvc;
using test.Models.Entity;
using test.Models;
using test.Services;
using test.Models.DTOs.Response.Mails;
using test.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;
using AutoMapper;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using test.Models.DTOs;
using OfficeOpenXml;
using test.Extensions;
using test.Attributes;
using test.Services.Hubs;
namespace test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMailNotificationService _notificationService;

        public MailsController(ApplicationDbContext context, IMapper mapper, IMailNotificationService notificationService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        // Add private method to get user claims
        // private (string userEmail, List<string> rolesList) GetUserClaims()
        // {
        //     var roles = User.Claims.FirstOrDefault(c => c.Type == "roles")?.Value ?? "";
        //     var userEmail = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        //     var rolesList = roles.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        //     return (userEmail, rolesList);
        // }

        // GET: api/Mails
        // [HttpGet]
        // public async Task<ActionResult<ApiResponse<PaginatedList<Mail>>>> GetMails(
        //     int? page, 
        //     string? id,
        //     string? email,
        //     bool? isSend,
        //     string? timeType,
        //     string? sendStatus,
        //     string? emailCc,
        //     string? emailBcc,
        //     DateTime? fromDate,
        //     DateTime? toDate)
        // {
        //     try 
        //     {
        //         var (userEmail, rolesList) = GetUserClaims();
        //         int pageSize = 6;
        //         int pageIndex = page ?? 1;
                
        //         // Start with base query
        //         var query = _context.Mail.AsQueryable();

        //         // Apply role-based filtering first
        //         if (!rolesList.Contains("admin"))
        //         {
        //             query = query.Where(m => m.Email == userEmail);
        //         }


        //         // Then apply other filters...
        //         if (!string.IsNullOrEmpty(id))
        //         {
        //             query = query.Where(m => m.Id.ToString().Equals(id));
        //         }

        //         if (!string.IsNullOrEmpty(email))
        //         {
        //             query = query.Where(m => m.Email != null && m.Email.Equals(email));
        //         }

        //         if (isSend.HasValue)
        //         {
        //             query = query.Where(m => m.IsSend == isSend.Value);
        //         }

        //         if (!string.IsNullOrEmpty(sendStatus))
        //         {
        //             query = query.Where(m => m.SentStatus != null && m.SentStatus.Contains(sendStatus));
        //         }

        //         if (!string.IsNullOrEmpty(emailCc))
        //         {
        //             var emailCcList = emailCc.Split(';', StringSplitOptions.RemoveEmptyEntries);
        //             query = query.Where(m => m.EmailCc != null && emailCcList.Any(cc => m.EmailCc.Contains(";" + cc + ";")));
        //         }

        //         if (!string.IsNullOrEmpty(emailBcc))
        //         {
        //             var emailBccList = emailBcc.Split(';', StringSplitOptions.RemoveEmptyEntries);
        //             query = query.Where(m => m.EmailBcc != null && emailBccList.Any(bcc => m.EmailBcc.Contains(";" + bcc + ";")));
        //         }
    
        //         if (!string.IsNullOrEmpty(timeType) && fromDate.HasValue && toDate.HasValue)
        //         {
        //             if (fromDate.Value <= toDate.Value)
        //             {
        //                 if (timeType == "createTime")
        //                 {
        //                     query = query.Where(m => m.CreateTime >= fromDate.Value &&
        //                                            m.CreateTime <= toDate.Value);
        //                 }
        //                 else if (timeType == "sendTime")
        //                 {
        //                     query = query.Where(m => m.SendTime >= fromDate.Value &&
        //                                            m.SendTime <= toDate.Value);
        //                 }
        //             }
        //         }

        //         var totalItems = await query.CountAsync();
        //         var items = await query
        //             .OrderByDescending(m => m.Id)
        //             .Skip((pageIndex - 1) * pageSize)
        //             .Take(pageSize)
        //             .ToListAsync();

        //         var result = new
        //         {
        //             items = items,
        //             pageIndex = pageIndex,
        //             totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
        //             totalItems = totalItems,
        //             hasPreviousPage = pageIndex > 1,
        //             hasNextPage = pageIndex < (int)Math.Ceiling(totalItems / (double)pageSize)
        //         };

        //         return Ok(new ApiResponse<object>
        //         {
        //             Success = true,
        //             Data = result
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, new ApiResponse<object>
        //         {
        //             Success = false,
        //             Message = ex.Message
        //         });
        //     }
        // }

        [HttpGet]
        [ModulePermission("Mails", requireRead: true)]
        public async Task<ActionResult<ApiResponse<MailRes>>> GetMails(
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
            try
            {   
                var ls = new MailRes();
                var (claimsId, userEmail, rolesList) = this.GetUserClaims();
                int pageSize = 6;
                int pageIndex = page ?? 1;
                DataTable dt = new();
                if (!rolesList.Contains("admin"))
                {
                    email = userEmail;
                }
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "GetMails";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@Page", pageIndex));
                    command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
                    command.Parameters.Add(new SqlParameter("@Id", id));
                    command.Parameters.Add(new SqlParameter("@Email", email));
                    command.Parameters.Add(new SqlParameter("@IsSend", isSend));
                    command.Parameters.Add(new SqlParameter("@TimeType", timeType));
                    command.Parameters.Add(new SqlParameter("@SendStatus", sendStatus));
                    command.Parameters.Add(new SqlParameter("@EmailCc", emailCc));
                    command.Parameters.Add(new SqlParameter("@EmailBcc", emailBcc));
                    command.Parameters.Add(new SqlParameter("@FromDate", fromDate));
                    command.Parameters.Add(new SqlParameter("@ToDate", toDate));

                    // tham số output để lấy TotalItems
                    var totalItemsParam = new SqlParameter("@TotalItems", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(totalItemsParam);
                    await _context.Database.OpenConnectionAsync();
                    using var result = await command.ExecuteReaderAsync();
                    dt.Load(result);

                    ls.Items = dt.ConvertDataTable<Mail>();
                    ls.TotalItems = (int)(totalItemsParam.Value ?? 0);
                    ls.PageIndex = pageIndex;
                    ls.PageSize = pageSize;

                    return Ok(new ApiResponse<MailRes>
                    {
                        Success = true,
                        Message = "Successfully retrieved mails.",
                        Data = ls
                    });
                }  
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<MailRes>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("{id}")]
        [ModulePermission("Mails", requireRead: true)]
        public async Task<ActionResult<ApiResponse<Mail>>> GetMail(long id)
        {
            try
            {
                var (claimsId, userEmail, rolesList) = this.GetUserClaims();
                // Get the mail
                var mail = await _context.Mail.FindAsync(id);
                
                // if (mail == null)
                // {
                //     return NotFound(new ApiResponse<Mail> 
                //     { 
                //         Success = false, 
                //         Message = "Mail not found" 
                //     });
                // }

                // Check authorization
                if (mail == null || (!rolesList.Contains("admin") && mail.Email != userEmail))
                {
                    return StatusCode(403, new ApiResponse<Mail> 
                    {   Success = false, 
                        Message = "Access denied" ,
                        Data = null
                    });
                }

                return Ok(new ApiResponse<Mail>
                {
                    Success = true,
                    Data = mail
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Mail>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [ModulePermission("Mails", requireCreate: true)]
        public async Task<ActionResult<ApiResponse<Mail>>> CreateMail(Mail mail)
        {
            if (!string.IsNullOrEmpty(mail.EmailCc))
            {
                mail.EmailCc = ";" + mail.EmailCc.Replace(",", ";").Trim(';') + ";";
            }
            
            if (!string.IsNullOrEmpty(mail.EmailBcc))
            {
                mail.EmailBcc = ";" + mail.EmailBcc.Replace(",", ";").Trim(';') + ";";
            }

            await _context.Mail.AddAsync(mail);
            await _context.SaveChangesAsync();   

            return Ok(new ApiResponse<Mail> { Success = true, Data = mail });
        }

        [HttpPut("{id}")]
        [ModulePermission("Mails", requireUpdate: true)]
        public async Task<ActionResult<ApiResponse<Mail>>> UpdateMail(long id, Mail mail)
        {
            try
            {
                var (claimsId, userEmail, rolesList) = this.GetUserClaims();
                var existingMail = await _context.Mail.FindAsync(id);

                // if (existingMail == null)
                // {
                //     return NotFound(new ApiResponse<Mail> 
                //     { 
                //         Success = false, 
                //         Message = "Mail not found" 
                //     });
                // }

                // Check authorization
                if (existingMail == null || (!rolesList.Contains("admin") && existingMail.Email != userEmail))
                {
                    return StatusCode(403, new ApiResponse<Mail>
                    {
                        Success = false,
                        Message = "You don't have permission to edit this mail",
                        Data = null
                    });
                }

                if (!string.IsNullOrEmpty(mail.EmailCc))
                {
                    mail.EmailCc = ";" + mail.EmailCc.Replace(",", ";").Trim(';') + ";";
                }
                
                if (!string.IsNullOrEmpty(mail.EmailBcc))
                {
                    mail.EmailBcc = ";" + mail.EmailBcc.Replace(",", ";").Trim(';') + ";";
                }

                _mapper.Map(mail, existingMail);
                await _context.SaveChangesAsync();

                await _notificationService.NotifyMailUpdated(id.ToString());


                return Ok(new ApiResponse<Mail>
                {
                    Success = true,
                    Data = existingMail
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Mail>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [ModulePermission("Mails", requireDelete: true)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMail(long id)
        {
            var mail = await _context.Mail.FindAsync(id);
            if (mail == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Mail not found" });
            }

            _context.Mail.Remove(mail);
            await _context.SaveChangesAsync();

            await _notificationService.NotifyMailDeleted(id.ToString());

            return Ok(new ApiResponse<object> { Success = true });
        }

        [HttpPost("deleteMultiple")]
        [ModulePermission("Mails", requireDelete: true)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMultiple([FromBody] List<long> ids)
        {
            var mails = await _context.Mail.Where(m => ids.Contains(m.Id)).ToListAsync();
            _context.Mail.RemoveRange(mails);
            await _context.SaveChangesAsync();
            await _notificationService.NotifyMailDeleted(ids.ToString());
            return Ok(new ApiResponse<object> { Success = true });
        }

        [HttpGet("filter-options")]
        public async Task<ActionResult<ApiResponse<object>>> GetFilterOptions()
        {
            try
            {
                var (claimsId, userEmail, rolesList) = this.GetUserClaims();
                // Base query: Fetch necessary data only
                var query = _context.Mail.AsQueryable();

                if (!rolesList.Contains("admin"))
                {
                    if (string.IsNullOrEmpty(userEmail))
                        return Forbid(); // Return 403 if no email claim found

                    query = query.Where(m => m.Email == userEmail);
                }

                var data = await query
                    .Select(m => new
                    {
                        Id = m.Id.ToString(),
                        Email = m.Email,
                        EmailCc = m.EmailCc,
                        EmailBcc = m.EmailBcc
                    })
                    .ToListAsync();

                var filterOptions = new
                {
                    ids = data
                        .Where(m => !string.IsNullOrEmpty(m.Id))
                        .Select(m => m.Id)
                        .Distinct()
                        .ToList(),

                    emails = !rolesList.Contains("admin")
                        ? new List<string> { userEmail }
                        : data
                            .Where(m => !string.IsNullOrEmpty(m.Email))
                            .Select(m => m.Email)
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

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = filterOptions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

    }
}
