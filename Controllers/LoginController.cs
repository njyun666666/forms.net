using FormsNet.Common;
using FormsNet.Models.Login;
using FormsNet.Models.Organization;
using FormsNet.Services.IServices;
using FormsNet.ViewModel.Auth;
using FormsNet.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormsNet.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private ILogger<LoginController> _logger;
		private ILoginService _loginService;
		private IOrganizationService _organizationService;
		private IAuthService _authService;
		private IMyService _myService;
		private IHttpContextAccessor _accessor;


		public LoginController(IHttpContextAccessor accessor, ILogger<LoginController> logger, ILoginService loginService, IOrganizationService organizationService, IAuthService authService, IMyService myService)
		{
			_logger = logger;
			_loginService = loginService;
			_organizationService = organizationService;
			_authService = authService;
			_myService = myService;
			_accessor = accessor;
		}
		[HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			// 驗證model
			if (!ModelState.IsValid)
			{
				return Ok(new ResponseViewModel(ResponseCode.param_error,
					CommonTools.GetParamValidErrorMessage(ModelState)));
			}

			model.Account = model.Account.Trim();
			model.Password = CommonTools.ComputeHMACSHA256(model.Password.Trim(), _myService.BackEndV2_Key());



			// 檢查帳號/密碼，取得人員資料
			List<OrgAccountInfoCheckModel> list = await _loginService.CheckAccountPassword(model);

			ResponseViewModel checkModel = await _authService.CheckAccountEnable(list);

			if (checkModel.Code != (int)ResponseCode.success)
			{
				return Ok(checkModel);
			}

			string UID = list.FirstOrDefault().UID;

			AuthTokenViewModel authToken = new AuthTokenViewModel();
			authToken.UID = UID;
			authToken.TokenKey = Guid.NewGuid().ToString().Replace("-", "");

			await _loginService.AddLoginLog(authToken, CommonTools.Userip_Get(_accessor.HttpContext));

			LoginViewModel loginView = new LoginViewModel();
			loginView.Token = _authService.TokenEncrypt(authToken);

			loginView.InfoJsonString = CommonTools.Base64Encode(JsonSerializer.Serialize(await _loginService.GetInfo(UID)));

			return Ok(new OKResponse() { Data = loginView });
		}
		[HttpPost]
		public async Task<IActionResult> ResetPw([FromBody] ResetPwModel model)
		{
			return Ok(await _loginService.ResetPw(model.Account));
		}
		[HttpPost]
		public async Task<IActionResult> GetInfo([FromHeader] string UID)
		{
			return Ok(new
			{
				InfoJsonString = CommonTools.Base64Encode(JsonSerializer.Serialize(await _loginService.GetInfo(UID)))
			});
		}
		[HttpPost]
		public async Task<IActionResult> ChangeUser(ChangeUserModel model)
		{
			string UID = model.UID;

			AuthTokenViewModel authToken = new AuthTokenViewModel();
			authToken.UID = UID;
			authToken.TokenKey = Guid.NewGuid().ToString().Replace("-", "");

			await _loginService.AddLoginLog(authToken, CommonTools.Userip_Get(_accessor.HttpContext));

			LoginViewModel loginView = new LoginViewModel();
			loginView.Token = _authService.TokenEncrypt(authToken);

			loginView.InfoJsonString = CommonTools.Base64Encode(JsonSerializer.Serialize(await _loginService.GetInfo(UID)));

			return Ok(new OKResponse() { Data = loginView });
		}
	}
}
