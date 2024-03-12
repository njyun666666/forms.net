using FormsNet.Filters;
using FormsNet.Models.Organization;
using FormsNet.Services.IServices;
using FormsNet.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrganizationController : Controller
    {
        private ILogger<OrganizationController> _logger;
        private IOrganizationService _organizationService;
        public OrganizationController(ILogger<OrganizationController> logger, IOrganizationService organizationService)
        {
            _logger = logger;
            _organizationService = organizationService;
        }
        /// <summary>
        /// 取得人員清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OrgAccountList(OrgAccountListGetModel model)
        {
            return Ok(new OKResponse { Data = await _organizationService.OrgAccountList(model) });
        }
        /// <summary>
        /// 新增/修改部門
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 4 })]
        [HttpPost]
        public async Task<IActionResult> AddDeptInfoStruct([FromHeader] string uid, [FromBody] OrgDeptInfoStructAddModel model)
        {
            // 檢查參數
            if (string.IsNullOrWhiteSpace(model.DeptName.Trim()))
            {
                return Ok(new FailResponse("請輸入部門名稱"));
            }

            return Ok(await _organizationService.AddDeptInfoStruct(uid, model));
        }
        /// <summary>
        /// 取得部門層級
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDeptLevel(OrgDeptLevelGetModel model)
        {
            return Ok(new OKResponse { Data = await _organizationService.GetDeptLevel(model) });
        }
        /// <summary>
        /// 組織選取器 取得部門清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OrgPickerGetDeptInfoList()
        {
            return Ok(new OKResponse { Data = await _organizationService.OrgPicker_GetDeptInfo() });
        }
        /// <summary>
        /// 組織選取器 取得人員清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OrgPickerGetUserInfoList([FromHeader]string uid, [FromBody] OrgPickerAccountInfoGetModel model)
        {
            return Ok(new OKResponse { Data = await _organizationService.OrgPicker_GetUserInfo(uid, model) });
        }
        /// <summary>
        /// 取得職稱清單
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetTitleList(OrgTitleGetModel model)
        {
            return Ok(new OKResponse { Data = await _organizationService.GetTitleList(model) });
        }
        /// <summary>
        /// 取得公司別清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetCompanyTypeList()
        {
            return Ok(new OKResponse { Data = await _organizationService.GetCompanyTypeList() });
        }
        /// <summary>
        /// 新增人員
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 4 })]
        [HttpPost]
        public async Task<IActionResult> AddUserInfoStruct([FromHeader] string uid, [FromBody] OrgAccountInfoStructAddModel model)
        {
            return Ok(await _organizationService.AddAccountInfoStruct(uid, model));
        }
        /// <summary>
        /// 取得組織樹
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetOrgNode()
        {
            return Ok(new OKResponse() { Data = await _organizationService.GetOrgNode() });
        }
        /// <summary>
        /// 取得部門清單
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDeptList()
        {
            return Ok(new OKResponse() { Data = await _organizationService.GetDeptList() });
        }
        [HttpPost]
        public async Task<IActionResult> GetDeptInfoStruct(DeptInfoStructEditModel model)
        {
            return Ok(new OKResponse() { Data = await _organizationService.GetDeptInfoStruct(model.DeptID) });
        }
        [HttpPost]
        public async Task<IActionResult> GetUserInfoStruct(OrgAccountInfoModel model)
        {
            return Ok(new OKResponse() { Data = await _organizationService.GetAccountInfo(model.UID) });
        }
        /// <summary>
        ///  修改人員資訊
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 4 })]
        [HttpPost]
        public async Task<IActionResult> EditUserInfo([FromHeader] string uid, [FromBody] OrgAccountInfoModel model)
        {
            // 檢查參數
            if (string.IsNullOrWhiteSpace(model.Name.Trim()))
            {
                return Ok(new FailResponse("請輸入 姓名"));
            }

            if (string.IsNullOrWhiteSpace(model.Email.Trim()))
            {
                return Ok(new FailResponse("請輸入 Email"));
            }

            if (string.IsNullOrWhiteSpace(model.EmployeeID.Trim()))
            {
                return Ok(new FailResponse("請輸入 員工編號"));
            }


            return Ok(await _organizationService.EditUserInfo(uid, model));

        }
        /// <summary>
        /// 新增/修改人員所屬部門
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 4 })]
        [HttpPost]
        public async Task<IActionResult> AddUserStruct([FromHeader] string uid, [FromBody] OrgAccountStructAddModel model)
        {
            return Ok(await _organizationService.AddAccountStruct(uid, model));
        }
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 10 })]
        [HttpPost]
        public async Task<IActionResult> AddDeptLevel([FromHeader] string uid, [FromBody] OrgDeptLevelAddModel model)
        {
            // 檢查參數
            if (string.IsNullOrWhiteSpace(model.LevelName.Trim()))
            {
                return Ok(new FailResponse("請輸入 部門層級"));
            }

            return Ok(await _organizationService.AddDeptLevel(uid, model));
        }
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 12 })]
        [HttpPost]
        public async Task<IActionResult> AddTitle([FromHeader] string uid, [FromBody] OrgTitleAddModel model)
        {
            // 檢查參數
            if (string.IsNullOrWhiteSpace(model.Title.Trim()))
            {
                return Ok(new FailResponse("請輸入 職稱"));
            }

            return Ok(await _organizationService.AddTitle(uid, model));
        }






    }
}
