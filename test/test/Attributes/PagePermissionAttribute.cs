// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;
// using Microsoft.EntityFrameworkCore;
// using test.Services;
// using test.Extensions;
// using test.Models.DTOs;
// using System.IdentityModel.Tokens.Jwt;

// namespace test.Attributes
// {
//     public class PagePermissionAttribute : ActionFilterAttribute
//     {
//         private readonly string _moduleName;
//         private readonly bool _requireCreate;
//         private readonly bool _requireRead;
//         private readonly bool _requireUpdate;
//         private readonly bool _requireDelete;

//         public PagePermissionAttribute(
//             string moduleName,
//             bool requireCreate = false,
//             bool requireRead = false,
//             bool requireUpdate = false,
//             bool requireDelete = false)
//         {
//             _moduleName = moduleName;
//             _requireCreate = requireCreate;
//             _requireRead = requireRead;
//             _requireUpdate = requireUpdate;
//             _requireDelete = requireDelete;
//         }

//         public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//         {
//             var controller = context.Controller as Controller;
//             Console.WriteLine("Controller: " + controller);
//             if (controller == null)
//             {
//                 context.Result = new RedirectToActionResult("Login", "Account", null);
//                 return;
//             }

//             // Lấy token từ header
//             var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
//             Console.WriteLine("Token: " + token);
//             if (string.IsNullOrEmpty(token))
//             {
//                 // Lưu current URL vào session để sau khi login có thể redirect lại
//                 var returnUrl = context.HttpContext.Request.Path.Value;
//                 context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
//                 return;
//             }

//             try
//             {
//                 var tokenHandler = new JwtSecurityTokenHandler();
//                 var jwtToken = tokenHandler.ReadJwtToken(token);

//                 var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
//                 if (string.IsNullOrEmpty(userId))
//                 {
//                     context.Result = new RedirectToActionResult("Login", "Account", null);
//                     return;
//                 }

//                 var dbContext = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();

//                 var userModule = await dbContext.UserModule
//                     .Include(um => um.Module)
//                     .FirstOrDefaultAsync(um => 
//                         um.UserId == int.Parse(userId) && 
//                         um.Module.Name == _moduleName);

//                 if (userModule == null)
//                 {
//                     context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
//                     return;
//                 }

//                 if ((_requireCreate && !userModule.CanCreate) ||
//                     (_requireRead && !userModule.CanRead) ||
//                     (_requireUpdate && !userModule.CanUpdate) ||
//                     (_requireDelete && !userModule.CanDelete))
//                 {
//                     context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
//                     return;
//                 }

//                 await next();
//             }
//             catch (Exception)
//             {
//                 context.Result = new RedirectToActionResult("Login", "Account", null);
//                 return;
//             }
//         }
//     }
// } 