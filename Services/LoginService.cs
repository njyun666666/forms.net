using FormsNet.Common;
using FormsNet.DB.IDB;
using FormsNet.Models.Login;
using FormsNet.Models.Organization;
using FormsNet.Services.IServices;
using FormsNet.ViewModel.Auth;
using FormsNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormsNet.Services
{
    public class LoginService : ILoginService
    {
        private IBackEndV2DB _backEndV2DB;
        private IAuthService _authService;
        private IMyService _myService;
        private ISendEmailService _sendEmailService;


        public LoginService(IBackEndV2DB backEndV2DB, IAuthService authService, IMyService myService, ISendEmailService sendEmailService)
        {
            _backEndV2DB = backEndV2DB;
            _authService = authService;
            _myService = myService;
            _sendEmailService = sendEmailService;
        }

        // <summary>
        // 新增登入Log
        // </summary>
        // <returns></returns>
        public async Task<int> AddLoginLog(AuthTokenViewModel authToken, string ip)
        {
            return await _backEndV2DB.AddLoginLog(authToken, ip);
        }

        public async Task<List<OrgAccountInfoCheckModel>> CheckAccountPassword(LoginModel model)
        {
            return await _backEndV2DB.CheckAccountPassword(model);
        }
        public async Task<ResponseViewModel> ResetPw(string account)
		{
            string email = await _backEndV2DB.GetEmailByAccount(account);

			if (string.IsNullOrWhiteSpace(email))
			{
                return new ResponseViewModel(ResponseCode.fail, "找不到帳號/Email");
			}

            ResetPwMailModel mailModel = new ResetPwMailModel() { Account = account, ExpireTime = DateTime.Now.AddMinutes(10) };
            string token = _authService.TokenEncrypt(mailModel);
            string url = $"https://test.com/auth/resetpw?token={token}";

            string subject = "重設密碼";
            string body = $"重設密碼連結10分鐘有效<br><a href='{url}' target='_blank'>{url}</a>";

            int sendResult = await _sendEmailService.SendEmail(new string[] { email }, subject, body);

            return new ResponseViewModel(ResponseCode.success,"請至Email收信，重設密碼連結10分鐘有效");
        }
        public async Task<LoginInfoModel> GetInfo(string uid)
        {
            List<OrgAccountInfoCheckModel> info = await _authService.OrgAccountInfoByUID(uid);

            return info.Select(x => new LoginInfoModel
            {
                UID=x.UID,
                Name=x.Name,
                PhotoURL=x.PhotoURL
            }).FirstOrDefault();
        }
    }
}
