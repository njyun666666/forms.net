using FormsNet.Models.Auth;
using FormsNet.Models.Organization;
using FormsNet.ViewModel.Auth;
using FormsNet.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices
{
    public interface IAuthService
    {
        public Task<List<OrgAccountInfoCheckModel>> OrgAccountInfoByUID(string uid);
        public Task<ResponseViewModel> CheckAccountEnable(List<OrgAccountInfoCheckModel> list);
        public Task<bool> Check(string uid, AuthCheckModel checkModel);
        public string TokenEncrypt(object model);
        public Task<bool> CheckTokenKey(AuthTokenViewModel model);
        public T TokenDecrypt<T>(string data);
        public bool IsAdmin(string uid);
        public Task<List<MenuAuthListModel>> GetUserMenuAuthList(string uid, MenuAuthListGetModel model);
        public Task<ResponseCode> SetUserMenuAuth(string editor, MenuAuthSetModel model);
        public Task<bool> AuthCheck(int menuID, string uid);
    }
}
