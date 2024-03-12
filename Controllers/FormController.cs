using FormsNet.Enums;
using FormsNet.Filters;
using FormsNet.Models.Form;
using FormsNet.Models.Form.SR;
using FormsNet.Models.Sign;
using FormsNet.Services.IServices.Form;
using FormsNet.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormsNet.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FormController : ControllerBase
    {
        IFormService _formService;
        public FormController(IFormService formService)
        {
            _formService = formService;
        }
        /// <summary>
        /// 取得啟用表單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetForms()
        {
            return Ok(await _formService.GetForms());
        }
        [HttpPost]
        public async Task<IActionResult> GetBaseData([FromHeader] string UID, BaseDataRequestModel model)
        {
            return Ok(await _formService.formBaseData(UID, model));
        }
        [HttpPost]
        [TypeFilter(typeof(FormAuthFilter))]
        public async Task<IActionResult> GetFormData(FormIDModel model)
        {
            return Ok(await _formService.GetFormData(model.FormID));
        }
        /// <summary>
        /// 刪除草稿
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteDraft([FromHeader] string uid, FormIDModel model)
        {
            int result = await _formService.DeleteDraft(uid, model.FormID);

            if (result == 0)
            {
                return Ok(new FailResponse());
            }

            return Ok(new OKResponse());
        }
        /// <summary>
        /// 儲存草稿/申請表單
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="jsonElement"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Applicant([FromHeader] string UID, [FromBody] JsonElement jsonElement)
        {
            return Ok(await _formService.Applicant(UID, jsonElement));
        }
        /// <summary>
        /// 草稿清單
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDraftList([FromHeader] string UID)
        {
            return Ok(await _formService.GetDraftList(UID));
        }
        [HttpGet("{formClass}")]
        [TypeFilter(typeof(FormClassEnabledFilter))]
        public async Task<IActionResult> FormClassEnabled(string formClass)
        {
            return Ok(new OKResponse());
        }
        [HttpPost]
        [TypeFilter(typeof(FormAuthFilter))]
        public async Task<IActionResult> FormAuth([FromHeader] string uid, FormIDModel model)
        {
            return Ok(new OKResponse());
        }
        /// <summary>
        /// 草稿權限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(DraftAuthFilter))]
        public async Task<IActionResult> DraftAuth(DraftAuthModel model)
        {
            return Ok(new OKResponse());
        }
        [HttpPost]
        [TypeFilter(typeof(SignAuthFilter))]
        public async Task<IActionResult> SignAuth(SignAuthModel model)
        {
            return Ok(new OKResponse());
        }
        /// <summary>
        /// 取得待簽核清單
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetApproverList([FromHeader] string UID)
        {
            return Ok(await _formService.GetApproverList(UID));
        }
        /// <summary>
        /// 取得申請表單清單
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetApplicantFormList([FromHeader] string UID)
        {
            return Ok(await _formService.GetApplicantFormList(UID));
        }
        /// <summary>
        /// 取得此筆簽核資料
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(SignAuthFilter))]
        public async Task<IActionResult> GetSignForm([FromHeader] string UID, SignAuthModel model)
        {
            return Ok(await _formService.GetSignForm(model.ID));
        }
        /// <summary>
        /// 取得簽核選項
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetSignOption([FromHeader] string UID, GetSignOptionModel model)
        {
            return Ok(await _formService.GetSignOption(UID, model));
        }
        /// <summary>
        /// 簽核
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Sign([FromHeader] string UID, SignModel model)
        {
            // 檢查權限
            if (!await _formService.SignAuth(UID, new SignAuthModel() { ID = model.ID, FormID = model.FormID }))
            {
                return Ok(new NoAuthResponse());
            }

            return Ok(await _formService.Sign(UID, model));
        }
        /// <summary>
        /// 修改表單狀態
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SignSetResult([FromHeader] string UID, SignFormModel model)
        {
            // 檢查權限
            switch (model.SignOptionID)
            {
                // 申請人修改選項 撤回
                case SignOptionEnum.applicant_edit_cancel:

                    if (!await _formService.SignCancelAuth(UID, new SignAuthModel() { ID = model.ID, FormID = model.FormID }))
                    {
                        return Ok(new NoAuthResponse());
                    }
                    break;

                default:

                    if (!await _formService.CheckAuthFormClassEditResult(UID, model.FormID))
                    {
                        return Ok(new NoAuthResponse());
                    }

                    break;
            }

            return Ok(await _formService.SignSetResult(UID, model));
        }
        /// <summary>
        /// 取得簽核紀錄
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SignLogList([FromHeader] string UID, SignLogRequestModel model)
        {
            return Ok(await _formService.SignLogList(model.FormID));
        }

        #region FormAuth Setting

        /// <summary>
        /// 取得使用者表單權限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 18 })]
        public async Task<IActionResult> FormAuthSetting(FormAuthSettingReqeustModel model)
        {
            return Ok(await _formService.FormAuthSetting(model));
        }
        /// <summary>
        /// 修改使用者表單權限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 18 })]
        public async Task<IActionResult> FormAuthEdit([FromHeader] string UID, FormAuthEditModel model)
        {

            if (await _formService.EditFormAuth(UID, model) == 0)
            {
                return Ok(new FailResponse());
            }

            return Ok(new OKResponse());
        }
        #endregion
    }
}
