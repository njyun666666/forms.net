using FormsNet.DB.IDB;
using FormsNet.Models.Organization;
using FormsNet.Services.IServices;
using FormsNet.ViewModel.Organization;
using FormsNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormsNet.Services
{
    public class OrganizationService : IOrganizationService
    {
        private IBackEndV2DB _backEndV2DB;
        private IAuthService _authService;

        List<OrgDeptInfoStructModel> NodeDeptList;
        List<OrgAccountInfoStructModel> NodeUserList;

        public OrganizationService(IBackEndV2DB backEndV2DB, IAuthService authService)
        {
            _backEndV2DB = backEndV2DB;
            _authService = authService;
        }
        /// <summary>
        /// 取得人員清單
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrgAccountListModel>> OrgAccountList(OrgAccountListGetModel model)
        {
            return await _backEndV2DB.OrgAccountList(model);
        }
        public async Task<List<OrgDeptLevelModel>> GetDeptLevel(OrgDeptLevelGetModel model)
        {
            return await _backEndV2DB.GetDeptLevel(model);
        }
        public async Task<ResponseViewModel> AddDeptInfoStruct(string uid, OrgDeptInfoStructAddModel model)
        {
            return await _backEndV2DB.AddDeptInfoStruct(uid, model);
        }
        public async Task<List<OrgPickerDeptInfoModel>> OrgPicker_GetDeptInfo()
        {
            return await _backEndV2DB.OrgPicker_GetDeptInfo();
        }
        public async Task<List<OrgPickerUserInfoModel>> OrgPicker_GetUserInfo(string uid, OrgPickerAccountInfoGetModel model)
        {
            return await _backEndV2DB.OrgPicker_GetUserInfo(uid, model);
        }
        public async Task<List<OrgTitleModel>> GetTitleList(OrgTitleGetModel model)
        {
            return await _backEndV2DB.GetTitleList(model);
        }
        public async Task<List<OrgCompanyTypeModel>> GetCompanyTypeList()
        {
            return await _backEndV2DB.GetCompanyTypeList();
        }
        public async Task<ResponseViewModel> AddAccountInfoStruct(string uid, OrgAccountInfoStructAddModel model)
        {
            model.Main = 1;
            return await _backEndV2DB.AddAccountInfoStruct(uid, model);
        }
        /// <summary>
        /// 取得組織樹
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrgNodeModel>> GetOrgNode()
        {
            NodeDeptList = await _backEndV2DB.OrgDeptInfoStruct();
            NodeUserList = await _backEndV2DB.OrgAccountInfoStruct();
            return SetOrgNode(null);
        }
        /// <summary>
        /// 組合組織樹
        /// </summary>
        /// <param name="deptID"></param>
        /// <returns></returns>
        public List<OrgNodeModel> SetOrgNode(string deptID)
        {
            List<OrgNodeModel> targetDept = NodeDeptList.Where(x => x.ParentDept == deptID)
                .OrderBy(x => x.Sort)
                .Select(x => new OrgNodeModel
                {
                    id = x.DeptID,
                    name = x.DeptName,
                    type = "dept",
                    expand = x.Expand
                }).ToList();


            targetDept.ForEach(x =>
            {
                x.children = (from u in NodeUserList
                              where u.DeptID == x.id
                              orderby u.SignApprover descending, u.Level descending, u.Name
                              select new OrgNodeModel
                              {
                                  id = u.UID,
                                  name = u.Name,
                                  type = "user",
                                  manager = u.SignApprover,
                                  deptID = u.DeptID
                              }
                              ).ToList();


                x.children.AddRange(SetOrgNode(x.id));
            });

            return targetDept;
        }
        public async Task<List<OrgDeptListModel>> GetDeptList()
        {
            return await _backEndV2DB.GetDeptList();
        }
        public async Task<DeptInfoStructEditModel> GetDeptInfoStruct(string deptID)
        {
            return await _backEndV2DB.GetDeptInfoStruct(deptID);
        }

        public async Task<OrgAccountInfoViewModel> GetAccountInfo(string uid)
        {
            OrgAccountInfoViewModel result = new OrgAccountInfoViewModel();
            OrgAccountInfoModel infoModel = await _backEndV2DB.GetAccountInfo(uid);
            List<OrgAccountStructModel> list = await _backEndV2DB.GetAccountStructList(uid);

            result.UID = infoModel.UID;
            result.Name = infoModel.Name;
            result.Email = infoModel.Email;
            result.EmployeeID = infoModel.EmployeeID;
            result.Status = infoModel.Status;
            result.StructList = list;

            return result;
        }
        public async Task<ResponseViewModel> EditUserInfo(string uid, OrgAccountInfoModel model)
        {
            return await _backEndV2DB.EditUserInfo(uid, model);
        }
        public async Task<ResponseViewModel> AddAccountStruct(string uid, OrgAccountStructAddModel model)
        {
            ResponseViewModel result = await _backEndV2DB.AddAccountStruct(uid, model);

            if (result.Code == (int)ResponseCode.success)
            {
                result.Data = await _backEndV2DB.GetAccountStructList(model.UID);
                return result;
            }

            return result;
        }
        public async Task<ResponseViewModel> AddDeptLevel(string uid, OrgDeptLevelAddModel model)
        {
            return await _backEndV2DB.AddDeptLevel(uid, model);
        }
        public async Task<ResponseViewModel> AddTitle(string uid, OrgTitleAddModel model)
        {
            return await _backEndV2DB.AddTitle(uid, model);
        }



    }
}
