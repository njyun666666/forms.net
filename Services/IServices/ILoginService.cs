using FormsNet.Models.Login;
using FormsNet.Models.Organization;
using FormsNet.ViewModel.Auth;
using FormsNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices
{
    public interface ILoginService
    {
        public Task<int> AddLoginLog(AuthTokenViewModel authToken, string ip);
        public Task<List<OrgAccountInfoCheckModel>> CheckAccountPassword(LoginModel model);
        public Task<ResponseViewModel> ResetPw(string account);
        public Task<LoginInfoModel> GetInfo(string uid);
    }
}
