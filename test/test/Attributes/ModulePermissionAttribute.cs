using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using test.Services;
using test.Extensions;
using test.Models.DTOs;
namespace test.Attributes{
    
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
            var controller = context.Controller as ControllerBase;
            if (controller == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var (userId, userEmail, rolesList) = controller.GetUserClaims();
            if (string.IsNullOrEmpty(userId))
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
                    um.UserId == int.Parse(userId) && 
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

            if ((_requireCreate && !userModule.CanCreate) ||
                (_requireRead && !userModule.CanRead) ||
                (_requireUpdate && !userModule.CanUpdate) ||
                (_requireDelete && !userModule.CanDelete))
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