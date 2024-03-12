using FormsNet.Services.IServices;
using FormsNet.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormsNet.Middlewares
{
    public class AuthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICacheService _cache;
        private ILogger<AuthCheckMiddleware> _logger;
        private readonly IAuthService _authService;

        public AuthCheckMiddleware(RequestDelegate next, ICacheService cache,
            ILogger<AuthCheckMiddleware> logger, IAuthService authService)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
            _authService = authService;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            string url = httpContext.Request.Headers["url"].FirstOrDefault();
            string path = httpContext.Request.Path.Value;

            try
            {

                //if (path != "/api/Auth/Check")
                //{
                //    if (string.IsNullOrWhiteSpace(url) || !_authService.Check(url))
                //    {
                //        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new LoginFailResponse("權限不足")));
                //        return;
                //    }
                //}


            }
            catch (Exception ex)
            {
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new ExceptionResponse(ex.ToString())));
                return;
            }



            await _next(httpContext);

        }


    }
}
