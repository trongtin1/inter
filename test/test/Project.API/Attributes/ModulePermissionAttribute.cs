using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using test.Project.Application.DTOs;
using test.Project.Infrastructure.Data;
using test.Project.Infrastructure.Extensions;
namespace test.Project.API.Attributes{
    
    public class ModulePermissionAttribute : ActionFilterAttribute

    {
        private readonly string _moduleName;
        private readonly bool _requireCreate;
        private readonly bool _requireRead;
        private readonly bool _requireUpdate;
        private readonly bool _requireDelete;

        public ModulePermissionAttribute(
            string moduleName,
            bool requireCreate = false,
            bool requireRead = false,
            bool requireUpdate = false,
            bool requireDelete = false)
        {
            _moduleName = moduleName;
            _requireCreate = requireCreate;
            _requireRead = requireRead;
            _requireUpdate = requireUpdate;
            _requireDelete = requireDelete;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var contextService = context.HttpContext.RequestServices.GetService<IContextService>();
            
            var id = contextService.GetUserId();

            if (string.IsNullOrEmpty(id))
            {
                context.Result = new JsonResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Unauthorized access",
                    Data = null
                })
                { 
                    StatusCode = StatusCodes.Status401Unauthorized 
                };
                return;
            }

            var dbContext = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();

            var userModule = await dbContext.UserModule
                .Include(um => um.Module)
                .FirstOrDefaultAsync(um => 
                    um.UserId == int.Parse(id) && 
                    um.Module.Name == _moduleName);

            if (userModule == null)
            {
                context.Result = new JsonResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You don't have permission to access this module",
                    Data = null
                })
                { 
                    StatusCode = StatusCodes.Status403Forbidden 
                };
                return;
            }

            if (_requireCreate && !userModule.CanCreate ||
                _requireRead && !userModule.CanRead ||
                _requireUpdate && !userModule.CanUpdate ||
                _requireDelete && !userModule.CanDelete)
            {
                context.Result = new JsonResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You don't have sufficient permissions for this operation",
                    Data = null
                })
                { 
                    StatusCode = StatusCodes.Status403Forbidden 
                };
                return;
            }

            await next();
        }
    } 
}