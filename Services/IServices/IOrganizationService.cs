using FormsNet.Models.Organization;
using FormsNet.ViewModel.Organization;
using FormsNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices
{
    public interface IOrganizationService
    {
        public Task<List<OrgAccountListModel>> OrgAccountList(OrgAccountListGetModel model);
        public Task<List<OrgDeptLevelModel>> GetDeptLevel(OrgDeptLevelGetModel model);
        public Task<ResponseViewModel> AddDeptInfoStruct(string uid, OrgDeptInfoStructAddModel model);
        public Task<List<OrgPickerDeptInfoModel>> OrgPicker_GetDeptInfo();
        public Task<List<OrgTitleModel>> GetTitleList(OrgTitleGetModel model);
        public Task<List<OrgCompanyTypeModel>> GetCompanyTypeList();
        public Task<ResponseViewModel> AddAccountInfoStruct(string uid, OrgAccountInfoStructAddModel model);
        public Task<List<OrgNodeModel>> GetOrgNode();
        public Task<DeptInfoStructEditModel> GetDeptInfoStruct(string deptID);
        public Task<OrgAccountInfoViewModel> GetAccountInfo(string uid);
        public Task<ResponseViewModel> EditUserInfo(string uid, OrgAccountInfoModel model);
        public Task<List<OrgPickerUserInfoModel>> OrgPicker_GetUserInfo(string uid, OrgPickerAccountInfoGetModel model);
        public Task<ResponseViewModel> AddAccountStruct(string uid, OrgAccountStructAddModel model);
        public Task<List<OrgDeptListModel>> GetDeptList();
        public Task<ResponseViewModel> AddDeptLevel(string uid, OrgDeptLevelAddModel model);
        public Task<ResponseViewModel> AddTitle(string uid, OrgTitleAddModel model);
    }
}
