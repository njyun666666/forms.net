using FormsNet.Models.Auth;
using FormsNet.Models.Form;
using FormsNet.Models.Game;
using FormsNet.Models.Login;
using FormsNet.Models.Menu;
using FormsNet.Models.Organization;
using FormsNet.ViewModel.Auth;
using FormsNet.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB.IDB
{
    public interface IBackEndV2DB
    {
        public Task<int> AddLoginLog(AuthTokenViewModel authToken, string ip);
        public Task<bool> CheckTokenKey(AuthTokenViewModel model);
        public Task<List<MenuModel>> PermissionQuery(string UID);
        public Task<List<MenuModel>> Menu_Query();
        public Task<int> Menu_Add(MenuModel menuModel);
        public Task<int> Menu_Update(MenuModel menuModel);
        public Task<List<GameInfoModel>> GameInfoQuery();
        public Task<List<MenuTypeModel>> Menu_Type_Query();
        public Task<int> Menu_Type_Add(MenuTypeModel menuTypeModel);
        public Task<int> Menu_Type_Update(MenuTypeModel menuTypeModel);
        public Task<List<OrgAccountInfoCheckModel>> CheckAccountPassword(LoginModel model);
        public Task<List<OrgAccountInfoCheckModel>> OrgAccountInfoByUID(string uid);
        public Task<string> GetEmailByAccount(string account);
        public List<OrgAccountInfoCheckModel> GetAccountInfoByEmail(string email);
        public Task<List<OrgDeptLevelModel>> GetDeptLevel(OrgDeptLevelGetModel model);
        public Task<ResponseViewModel> AddDeptInfoStruct(string editor, OrgDeptInfoStructAddModel model);
        public Task<List<OrgPickerDeptInfoModel>> OrgPicker_GetDeptInfo();
        public Task<List<OrgPickerUserInfoModel>> OrgPicker_GetUserInfo(string manager, OrgPickerAccountInfoGetModel model);
        public Task<List<OrgTitleModel>> GetTitleList(OrgTitleGetModel model);
        public Task<List<OrgCompanyTypeModel>> GetCompanyTypeList();
        public Task<ResponseViewModel> AddAccountInfoStruct(string editor, OrgAccountInfoStructAddModel model);
        public Task<List<OrgDeptInfoStructModel>> OrgDeptInfoStruct();
        public Task<List<OrgAccountInfoStructModel>> OrgAccountInfoStruct();
        public Task<DeptInfoStructEditModel> GetDeptInfoStruct(string deptID);
        public Task<OrgAccountInfoModel> GetAccountInfo(string uid);
        public Task<List<OrgAccountStructModel>> GetAccountStructList(string uid);
        public Task<ResponseViewModel> EditUserInfo(string editor, OrgAccountInfoModel model);
        public Task<ResponseViewModel> AddAccountStruct(string editor, OrgAccountStructAddModel model);
        public Task<List<OrgDeptListModel>> GetDeptList();
        public Task<ResponseViewModel> AddDeptLevel(string editor, OrgDeptLevelAddModel model);
        public Task<ResponseViewModel> AddTitle(string editor, OrgTitleAddModel model);
        public Task<List<OrgAccountListModel>> OrgAccountList(OrgAccountListGetModel model);
        public Task<ResponseViewModel> AddAppInfo(string editor, GameInfoAddModel model);
        public Task<List<MenuAuthListModel>> GetUserMenuAuthList(string manager, MenuAuthListGetModel model, bool isAdmin);
        public Task<ResponseCode> SetUserMenuAuth(string editor, MenuAuthSetModel model);
        public Task<bool> AuthCheck(int menuID, string uid);
        public Task<int> GetMenuIDByURL(string url);
        public Task<List<GameInfoModel>> GameInfo_GetByAuth(int menuID, string uid);
    }
}
