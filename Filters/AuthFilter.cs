using FormsNet.Services.IServices;
using FormsNet.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Filters
{
    public class AuthFilter : IAsyncActionFilter
    {
        private IAuthService _authService;
        private int _menuID;


        public AuthFilter(IConfiguration config, IAuthService authService, int MenuID)
        {
            _authService = authService;
            _menuID = MenuID;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            string uid = context.HttpContext.Request.Headers["uid"];

            if (!await _authService.AuthCheck(_menuID, uid))
            {
                context.Result = new JsonResult(new NoAuthResponse());
                return;
            }

            await next();

        }
    }
    
    
}
