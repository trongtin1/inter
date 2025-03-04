
using Microsoft.AspNetCore.Mvc;

namespace test.Project.Application.ServiceInterfaces
{
    public interface IMailExcelService
    {
        Task<IActionResult> ExportToExcelAsync(
            string fileName,
            string? id = null,
            string? email = null,
            bool? isSend = null,
            string? timeType = null,
            string? sendStatus = null,
            string? emailCc = null,
            string? emailBcc = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        Task<IActionResult> ImportFromExcelAsync(IFormFile file);
    }
} 