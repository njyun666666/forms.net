using FormsNet.Models.Form;
using FormsNet.Services.IServices.Form;
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
    public class SignAuthFilter : IAsyncActionFilter
    {
        private IFormService _formService;


        public SignAuthFilter(IConfiguration config, IFormService formService)
        {
            _formService = formService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string uid = context.HttpContext.Request.Headers["uid"];

            SignAuthModel model = (SignAuthModel)context.ActionArguments["model"];

            if (!await _formService.SignAuth(uid, model))
            {
                context.Result = new JsonResult(new NoAuthResponse());
                return;
            }

            await next();

        }
    }
}
