using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using test.Project.Application.DTOs;
using test.Project.Application.ServiceInterfaces;
using test.Project.Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using test.Project.Infrastructure.Extensions;
namespace test.Project.Application.Services
{
    public class MailExcelService : IMailExcelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IContextService _contextService;
        public MailExcelService(IUnitOfWork unitOfWork, IContextService contextService)
        {
            _unitOfWork = unitOfWork;
            _contextService = contextService;
        }

        public async Task<IActionResult> ExportToExcelAsync(
            string fileName,
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
            try
            {
                var userEmail = _contextService.GetUserEmail();
                var rolesList = _contextService.GetUserRoles();

                var mails = await _unitOfWork.MailExcel.GetMailsForExportAsync(
                    userEmail, rolesList, id, email, isSend, timeType, sendStatus,
                    emailCc, emailBcc, fromDate, toDate);

                using var excelPackage = new ExcelPackage();
                // Thiết lập thông tin file
                excelPackage.Workbook.Properties.Author = "Mail System";
                excelPackage.Workbook.Properties.Title = "Mail Export";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = $"Mails_{DateTime.Now:yyyyMMdd}.xlsx";
                }
                else if (!fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    fileName += ".xlsx";
                }
                fileName = Path.GetFileName(fileName);

                var worksheet = excelPackage.Workbook.Worksheets.Add("Mails");
                SetupHeaders(worksheet);
                FillDataAndFormat(worksheet, mails);

                var result = new FileContentResult(
                    excelPackage.GetAsByteArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                )
                {
                    FileDownloadName = fileName
                };

                return result;
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                })
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<IActionResult> ImportFromExcelAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new BadRequestObjectResult(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "No file uploaded"
                    });
                }

                if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    return new BadRequestObjectResult(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Please upload an Excel file (.xlsx)"
                    });
                }

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                var successCount = 0;
                var errors = new List<string>();
                var mails = new List<Mail>();

                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension?.Rows ?? 0;

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

                        if (string.IsNullOrEmpty(mail.Email) || string.IsNullOrEmpty(mail.Subject))
                        {
                            errors.Add($"Row {row}: Email and Subject are required");
                            continue;
                        }

                        mails.Add(mail);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {row}: {ex.Message}");
                    }
                }

                if (mails.Any())
                {
                    await _unitOfWork.MailExcel.CreateMailsAsync(mails);
                }

                return new OkObjectResult(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"Successfully imported {successCount} mails. {errors.Count} errors occurred.",
                    Data = new
                    {
                        SuccessCount = successCount,
                        Errors = errors
                    }
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                })
                {
                    StatusCode = 500
                };
            }
        }

        private void SetupHeaders(ExcelWorksheet worksheet)
        {
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
            worksheet.Cells.AutoFitColumns(15);
        }
    }
} 