using FormsNet.Common;
using FormsNet.Services.IServices;
using FormsNet.ViewModel.Auth;
using FormsNet.ViewModels;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormsNet.Middlewares
{
	public class TokenCheckMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ICacheService _cache;
		private ILogger<TokenCheckMiddleware> _logger;
		private readonly IAuthService _authService;

		public TokenCheckMiddleware(RequestDelegate next, ICacheService cache,
			ILogger<TokenCheckMiddleware> logger, IAuthService authService)
		{
			_next = next;
			_cache = cache;
			_logger = logger;
			_authService = authService;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			string token = httpContext.Request.Headers["token"].FirstOrDefault();

			if (string.IsNullOrEmpty(token))
			{
				await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new LoginFailResponse("找不到token")));
			}
			else
			{

				try
				{

					AuthTokenViewModel authToken = _authService.TokenDecrypt<AuthTokenViewModel>(token);

					if (authToken == null)
					{
						await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new LoginFailResponse("token解密失敗")));
						return;
					}

					//if (!await _authService.CheckTokenKey(authToken))
					//{
					//    await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new LoginFailResponse("token檢查失敗")));
					//    return;
					//}

					//if (authToken.ExpiresDate < DateTime.Now)
					//{
					//    await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new LoginFailResponse("token過期")));
					//    return;
					//}

					// 取得人員資訊
					//List<OrgAccountInfoCheckModel> list = await _authService.OrgAccountInfoByUID(authToken.UID);

					// 檢查帳號啟用
					//ResponseViewModel checkModel = await _authService.CheckAccountEnable(list);

					//if (checkModel.Code != (int)ResponseCode.success)
					//{
					//	await httpContext.Response.WriteAsync(JsonSerializer.Serialize(checkModel));
					//	return;
					//}



					//Header加UID資訊
					AddHeader(httpContext, "UID", authToken.UID);
					//Header加IP資訊
					AddHeader(httpContext, "IP", CommonTools.Userip_Get(httpContext));

				}
				catch (Exception ex)
				{
					_logger.LogInformation(ex.Message);
					await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new LoginFailResponse(ex.Message)));
					return;
				}

				await _next(httpContext);
			}

		}

		private void AddHeader(HttpContext httpContext, string key, string value)
		{
			if (httpContext.Request.Headers.ContainsKey(key))
			{
				httpContext.Request.Headers.Remove(key);
			}
			httpContext.Request.Headers.Add(key, value);
		}

	}
}
