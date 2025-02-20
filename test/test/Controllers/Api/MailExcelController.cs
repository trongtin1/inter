using Microsoft.AspNetCore.Mvc;
using test.Models.Entity;
using test.Models;
using test.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using test.Models.DTOs;
using test.Extensions;
namespace test.Controllers.Api
{
    [Route("api/mail-excel")]
    [ApiController]
    [Authorize]
    public class MailExcelController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public MailExcelController(ApplicationDbContext context)
        {
            this.context = context;
        }

        private Stream CreateExcelFile(List<Mail> mails, Stream stream = null)
        {
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                // Thiết lập thông tin file
                excelPackage.Workbook.Properties.Author = "Mail System";
                excelPackage.Workbook.Properties.Title = "Mail Export";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                // Tạo worksheet
                var worksheet = excelPackage.Workbook.Worksheets.Add("Mails");
                
                // Thiết lập header
                SetupHeaders(worksheet);
                
                // Đổ dữ liệu và định dạng
                FillDataAndFormat(worksheet, mails);

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        private void SetupHeaders(ExcelWorksheet worksheet)
        {
            // Thiết lập headers
            var headers = new[] {
                "Id", "Email", "Subject", "Email Content", "File Attach",
                "Create By", "Create Time", "Is Send", "Send Time",
                "Sent Status", "Email CC", "Email BCC", "From Date",
                "To Date", "Location", "Mail Type", "Organizer",
                "Organizer Mail", "UID"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            // Định dạng header-
            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                range.Style.Font.Bold = true;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            }
        }

        private void FillDataAndFormat(ExcelWorksheet worksheet, List<Mail> mails)
        {
            int row = 2;
            foreach (var mail in mails)
            {
                worksheet.Cells[row, 1].Value = mail.Id;
                worksheet.Cells[row, 2].Value = mail.Email;
                worksheet.Cells[row, 3].Value = mail.Subject;
                worksheet.Cells[row, 4].Value = mail.EmailContent;
                worksheet.Cells[row, 5].Value = mail.FileAttach;
                worksheet.Cells[row, 6].Value = mail.CreateBy;
                worksheet.Cells[row, 7].Value = mail.CreateTime;
                worksheet.Cells[row, 8].Value = mail.IsSend;
                worksheet.Cells[row, 9].Value = mail.SendTime;
                worksheet.Cells[row, 10].Value = mail.SentStatus;
                worksheet.Cells[row, 11].Value = mail.EmailCc;
                worksheet.Cells[row, 12].Value = mail.EmailBcc;
                worksheet.Cells[row, 13].Value = mail.FromDate;
                worksheet.Cells[row, 14].Value = mail.ToDate;
                worksheet.Cells[row, 15].Value = mail.Location;
                worksheet.Cells[row, 16].Value = mail.MailType;
                worksheet.Cells[row, 17].Value = mail.Organizer;
                worksheet.Cells[row, 18].Value = mail.OrganizerMail;
                worksheet.Cells[row, 19].Value = mail.Uid;

                // Định dạng các cột datetime
                worksheet.Cells[row, 7].Style.Numberformat.Format = "dd/mm/yyyy hh:mm";
                worksheet.Cells[row, 9].Style.Numberformat.Format = "dd/mm/yyyy hh:mm";
                worksheet.Cells[row, 13].Style.Numberformat.Format = "dd/mm/yyyy hh:mm";
                worksheet.Cells[row, 14].Style.Numberformat.Format = "dd/mm/yyyy hh:mm";

                row++;
            }

            // Auto-fit tất cả các cột
            worksheet.Cells.AutoFitColumns(15);
        }

        private IQueryable<Mail> BuildMailQuery(
            string userEmail,
            List<string> rolesList,
            string? id = null,
            string? email = null,
            bool? isSend = null,
            string? timeType = null,
            string? sendStatus = null,
            string? emailCc = null,
            string? emailBcc = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = context.Mail.AsQueryable();

            // Apply role-based filtering
                if (!rolesList.Contains("admin"))
                {
                    query = query.Where(m => m.Email == userEmail);
                }
                // Then apply other filters...
                if (!string.IsNullOrEmpty(id))
                {
                    query = query.Where(m => m.Id.ToString().Equals(id));
                }

                if (!string.IsNullOrEmpty(email))
                {
                    query = query.Where(m => m.Email != null && m.Email.Equals(email));
                }

                if (isSend.HasValue)
                {
                    query = query.Where(m => m.IsSend == isSend.Value);
                }

                if (!string.IsNullOrEmpty(sendStatus))
                {
                    query = query.Where(m => m.SentStatus != null && m.SentStatus.Contains(sendStatus));
                }

                if (!string.IsNullOrEmpty(emailCc))
                {
                    var emailCcList = emailCc.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    query = query.Where(m => m.EmailCc != null && emailCcList.Any(cc => m.EmailCc.Contains(";" + cc + ";")));
                }

                if (!string.IsNullOrEmpty(emailBcc))
                {
                    var emailBccList = emailBcc.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    query = query.Where(m => m.EmailBcc != null && emailBccList.Any(bcc => m.EmailBcc.Contains(";" + bcc + ";")));
                }
    
                if (!string.IsNullOrEmpty(timeType) && fromDate.HasValue && toDate.HasValue)
                {
                    if (fromDate.Value <= toDate.Value)
                    {
                        if (timeType == "createTime")
                        {
                            query = query.Where(m => m.CreateTime >= fromDate.Value &&
                                                   m.CreateTime <= toDate.Value);
                        }
                        else if (timeType == "sendTime")
                        {
                            query = query.Where(m => m.SendTime >= fromDate.Value &&
                                                   m.SendTime <= toDate.Value);
                        }
                    }
                }

            return query;
        }

        [HttpGet("export")]
        public async Task<ActionResult> ExportToExcel(
            [FromQuery] string fileName,
            [FromQuery] string? id,
            [FromQuery] string? email,
            [FromQuery] bool? isSend,
            [FromQuery] string? timeType,
            [FromQuery] string? sendStatus,
            [FromQuery] string? emailCc,
            [FromQuery] string? emailBcc,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var (claimsId,userEmail, rolesList) = this.GetUserClaims();
                // Use the shared query builder
                var query = BuildMailQuery(
                    userEmail,
                    rolesList,
                    id,
                    email,
                    isSend,
                    timeType,
                    sendStatus,
                    emailCc,
                    emailBcc,
                    fromDate,
                    toDate
                );

                var mails = await query.ToListAsync();

                // Validate filename
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = $"Mails_{DateTime.Now:yyyyMMdd}.xlsx";
                }
                else if (!fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    fileName += ".xlsx";
                }

                fileName = Path.GetFileName(fileName);

                var stream = CreateExcelFile(mails);
                var buffer = stream as MemoryStream;

                Response.Headers["Content-Disposition"] = $"attachment; filename={fileName}";
                Response.Headers["X-Success"] = "true";
                Response.Headers["X-Message"] = "Successfully exported";

                return File(buffer.ToArray(), 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    fileName);
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

        [HttpPost("import")]
        public async Task<ActionResult<ApiResponse<object>>> ImportFromExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "No file uploaded" });

                if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "Please upload an Excel file (.xlsx)" });

                var successCount = 0;
                var errors = new List<string>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension?.Rows ?? 0;

                        // Bắt đầu từ row 2 (skip header)
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try 
                            {
                                var mail = new Mail
                                {
                                    Email = worksheet.Cells[row, 2].Value?.ToString(),
                                    Subject = worksheet.Cells[row, 3].Value?.ToString(),
                                    EmailContent = worksheet.Cells[row, 4].Value?.ToString(),
                                    FileAttach = worksheet.Cells[row, 5].Value?.ToString(),
                                    CreateBy = worksheet.Cells[row, 6].Value?.ToString(),
                                    CreateTime = worksheet.Cells[row, 7].GetValue<DateTime>(),
                                    IsSend = worksheet.Cells[row, 8].GetValue<bool>(),
                                    SendTime = worksheet.Cells[row, 9].GetValue<DateTime?>(),
                                    SentStatus = worksheet.Cells[row, 10].Value?.ToString(),
                                    EmailCc = worksheet.Cells[row, 11].Value?.ToString(),
                                    EmailBcc = worksheet.Cells[row, 12].Value?.ToString(),
                                    FromDate = worksheet.Cells[row, 13].GetValue<DateTime?>(),
                                    ToDate = worksheet.Cells[row, 14].GetValue<DateTime?>(),
                                    Location = worksheet.Cells[row, 15].Value?.ToString(),
                                    MailType = int.Parse(worksheet.Cells[row, 16].Value?.ToString() ?? "0"),
                                    Organizer = worksheet.Cells[row, 17].Value?.ToString(),
                                    OrganizerMail = worksheet.Cells[row, 18].Value?.ToString(),
                                    Uid = worksheet.Cells[row, 19].Value?.ToString()
                                };

                                // Kiểm tra các trường bắt buộc
                                if (string.IsNullOrEmpty(mail.Email) || string.IsNullOrEmpty(mail.Subject))
                                {
                                    errors.Add($"Row {row}: Email and Subject are required");
                                    continue;
                                }

                                // Gọi API CreateMail
                                var result = await context.Mail.AddAsync(mail);
                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Row {row}: {ex.Message}");
                            }
                        }
                    }
                }

                await context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"Successfully imported {successCount} mails. {errors.Count} errors occurred.",
                    Data = new { 
                        SuccessCount = successCount,
                        Errors = errors
                    }
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