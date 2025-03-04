using test.Project.Application.ServiceInterfaces;
using test.Project.Application.DTOs;

namespace test.Project.Application.Services
{
    public class TemplateService : ITemplateService
    {

        public TemplateService()
        {
            
        }

        public async Task<ApiResponse<List<string>>> GetTemplateList(string templateType)
        {
            var templates = new List<string>();
            string templateDirectory = Path.Combine("wwwroot", "Template", templateType);

            if (Directory.Exists(templateDirectory))
            {
                templates = Directory.GetFiles(templateDirectory, "*.txt")
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToList();
            }

            return new ApiResponse<List<string>>
            {
                Success = true,
                Message = "Template list retrieved successfully.",
                Data = templates
            };
        }

        public async Task<ApiResponse<string>> GetTemplateContent(string templateType, string templateName)
        {
            string templateDirectory = Path.Combine("wwwroot", "Template", templateType);
            string templatePath = Path.Combine(templateDirectory, templateName + ".txt");

            if (System.IO.File.Exists(templatePath))
            {
                var content = System.IO.File.ReadAllText(templatePath);
                // Chuyển đổi xuống dòng thành <br> để hiển thị HTML
                content = content.Replace("\r\n", "<br>").Replace("\n", "<br>");
                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Template found.",
                    Data = content
                };
            }

            return new ApiResponse<string>
            {
                Success = false,
                Message = "Template not found."
            };
        }
    }
} 