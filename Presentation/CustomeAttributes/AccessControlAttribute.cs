using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.CustomeAttributes
{
    public class AccessControlAttribute : ActionFilterAttribute
    {
        public dynamic Permission { get; set; }

        public AccessControlAttribute(string permission)
        {
            Permission = permission;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await base.OnActionExecutionAsync(context, next);
            //var s = context.HttpContext.User.Claims;

            //var _authService = context.HttpContext.RequestServices.GetService<IAuthService>();

            //var permissionResult = await _authService.CheckPermission("1234567890", Permission);

            //if (permissionResult)
            //    await base.OnActionExecutionAsync(context, next);
            //else
            //    context.Result = new BadRequestObjectResult("not access in action");
        }
    }
}
