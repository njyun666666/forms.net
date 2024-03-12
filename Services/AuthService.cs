using FormsNet.Common;
using FormsNet.DB.IDB;
using FormsNet.Models.Auth;
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
    public class AuthService : IAuthService
    {
        private readonly IMyService _myService;
        private IBackEndV2DB _backEndV2DB;
        private List<string> admin;

        public AuthService(IMyService myService, IBackEndV2DB backEndV2DB)
        {
            _myService = myService;
            _backEndV2DB = backEndV2DB;

            admin = new List<string>() {
                "48574694-b408-422c-858e-c6a2b8943ec8"
                };
        }
        /// <summary>
        /// 取得帳號資訊
        /// </summary>
        /// <param name="gid"></param>
        /// <returns></returns>
        public async Task<List<OrgAccountInfoCheckModel>> OrgAccountInfoByUID(string uid)
		{
            return await _backEndV2DB.OrgAccountInfoByUID(uid);
        }
        /// <summary>
        /// 判斷帳號啟用
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> CheckAccountEnable(List<OrgAccountInfoCheckModel> list)
        {
            // 帳號/密碼錯誤
            if (list == null || !list.Any())
            {
                return new ResponseViewModel(ResponseCode.login_fail, "帳號/密碼錯誤");
            }

            // 檢查帳號啟用
            if (list.Where(x => x.Status == 0).Any())
            {
                return new ResponseViewModel(ResponseCode.login_fail, "帳號尚未啟用");
            }

            // 帳號未設定所屬部門
            if (list.Where(x => x.Status == 1 && string.IsNullOrWhiteSpace(x.DeptID)).Any())
            {
                return new ResponseViewModel(ResponseCode.login_fail, "帳號未設定所屬部門");
            }

            // 檢查帳號停用
            if (list.Where(x => x.Status == -1).Any())
            {
                return new ResponseViewModel(ResponseCode.login_fail, "帳號停用");
            }

            return new ResponseViewModel(ResponseCode.success);
        }
        /// <summary>
        /// 檢查頁面權限
        /// </summary>
        /// <param name="checkModel"></param>
        /// <returns></returns>
        public async Task<bool> Check(string uid, AuthCheckModel checkModel)
        {
            return await AuthCheck(await _backEndV2DB.GetMenuIDByURL(checkModel.url), uid);
        }
        /// <summary>
        /// 檢查權限
        /// </summary>
        /// <param name="menuID"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<bool> AuthCheck(int menuID, string uid)
        {
            return await _backEndV2DB.AuthCheck(menuID, uid);
        }
        /// <summary>
        /// token 加密
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string TokenEncrypt(object model)
        {
            string guid = Guid.NewGuid().ToString().Replace("-", "").Substring(16).ToLower();
            string payload_str = JsonSerializer.Serialize(model);
            string payload = CommonTools.AESEncrypt(payload_str, _myService.BackEndV2_Key(), guid);
            string signature = CommonTools.ComputeHMACSHA256(payload, _myService.BackEndV2_Key());

            return $"{guid}.{payload}.{signature}";
        }
        public async Task<bool> CheckTokenKey(AuthTokenViewModel model)
		{
            return await _backEndV2DB.CheckTokenKey(model);
        }
        /// <summary>
        /// token 解密
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public T TokenDecrypt<T>(string data)
        {
            try
            {
                string[] data_arr = data.Split(".");
                string guid = data_arr[0];
                string payload = data_arr[1];
                string signature = data_arr[2];

                string _signature = CommonTools.ComputeHMACSHA256(payload, _myService.BackEndV2_Key());

                if (signature != _signature)
                {
                    return default(T);
                }

                string payload_str = CommonTools.AESDecrypt(payload, _myService.BackEndV2_Key(), guid);
                return JsonSerializer.Deserialize<T>(payload_str);
            }
            catch (Exception)
            {

            }

            return default(T);
        }
        /// <summary>
        /// uid是否為admin
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool IsAdmin(string uid)
        {
            return admin.Contains(uid);
        }
        /// <summary>
        /// 權限管理 取得使用者權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<List<MenuAuthListModel>> GetUserMenuAuthList(string uid, MenuAuthListGetModel model)
        {
            bool isAdmin = admin.Contains(uid) ? true : false;
            return await _backEndV2DB.GetUserMenuAuthList(uid, model, isAdmin);
        }
        /// <summary>
        /// 權限管理 修改使用者權限
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseCode> SetUserMenuAuth(string editor, MenuAuthSetModel model)
        {
            return await _backEndV2DB.SetUserMenuAuth(editor, model);
        }
        

    }
}
