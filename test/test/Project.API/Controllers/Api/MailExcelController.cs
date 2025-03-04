using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using test.Project.Application.ServiceInterfaces;
namespace test.Project.API.Controllers.Api
{
    [Route("api/mail-excel")]
    [ApiController]
    [Authorize]
    public class MailExcelController : ControllerBase
    {
        private readonly IMailExcelService _mailExcelService;

        public MailExcelController(IMailExcelService mailExcelService)
        {
            _mailExcelService = mailExcelService;
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportToExcel(
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
           
            return await _mailExcelService.ExportToExcelAsync(
                fileName, id, email, isSend, timeType,
                sendStatus, emailCc, emailBcc, fromDate, toDate);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            return await _mailExcelService.ImportFromExcelAsync(file);
        }
    }
} 