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
    public class FormClassEnabledFilter : IAsyncActionFilter
    {
        private IFormService _formService;


        public FormClassEnabledFilter(IConfiguration config, IFormService formService)
        {
            _formService = formService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string uid = context.HttpContext.Request.Headers["uid"];

            string formClass = (string)context.ActionArguments["formClass"];

            if (!await _formService.FormClassEnabled(formClass))
            {
                context.Result = new JsonResult(new NoAuthResponse());
                return;
            }

            await next();

        }
    }
}
