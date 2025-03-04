using Microsoft.AspNetCore.Mvc;
using test.Project.Application.DTOs;
using test.Project.Application.ServiceInterfaces;
namespace test.Project.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplatesController(ITemplateService templateService)
        {
            _templateService = templateService;
        }
        [HttpGet("{templateType}")]
        public async Task<ApiResponse<List<string>>> GetTemplates(string templateType)
        {
            return await _templateService.GetTemplateList(templateType);
        
        }
        [HttpGet("{templateType}/{templateName}")]
        public async Task<ApiResponse<string>> GetTemplateContent(string templateType, string templateName)
        {
            return await _templateService.GetTemplateContent(templateType, templateName);
        
        }
    }
}
