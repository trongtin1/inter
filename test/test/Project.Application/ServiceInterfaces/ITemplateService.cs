using test.Project.Application.DTOs;

namespace test.Project.Application.ServiceInterfaces
{
    public interface ITemplateService
    {
        Task<ApiResponse<List<string>>> GetTemplateList(string templateType);
        Task<ApiResponse<string>> GetTemplateContent(string templateType, string templateName); 
    }
} 