using FormsNet.Models.Form;
using FormsNet.Models.Search;
using FormsNet.Models.Sign;
using FormsNet.ViewModel.Form;
using FormsNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices.Form
{
    public interface IFormService
    {
        public Task<List<FormsViewModel>> GetForms(Int16 status = 1);
        public Task<FormBaseDataModel> formBaseData(string uid, BaseDataRequestModel model);
        public Task<object> GetFormData(string formID);
        public Task<int> DeleteDraft(string uid, string formID);
        public Task<ResponseViewModel> Applicant(string uid, JsonElement jsonElement);
        public Task<List<DraftListModel>> GetDraftList(string uid);
        public Task<bool> FormClassEnabled(string formClass);
        public Task<bool> FormAuth(string uid, string formID);
        public Task<bool> DraftAuth(string uid, string formID);
        public Task<bool> ApplicantAuth(string uid, string formID);
        public Task<bool> SignAuth(string uid, SignAuthModel model);
        public Task<bool> SignCancelAuth(string uid, SignAuthModel model);
        public Task<List<ApproverInfoModel>> GetApproverList(string uid);
        public Task<List<ApplicantInfoModel>> GetApplicantFormList(string uid);
        public Task<SignFormModel> GetSignForm(Int64 id);
        public Task<List<SignOptionModel>> GetSignOption(string UID, GetSignOptionModel model);
        public Task<ResponseViewModel> Sign(string uid, SignModel model);
        public Task<ResponseViewModel> SignSetResult(string uid, SignFormModel model);
        public Task<List<SignLogViewModel>> SignLogList(string FormID);
        public List<NowStepApproverViewModel> NowStepApprover_json(string json);
        public List<NowStepApproverViewModel> SetNowStepApprover(List<NowStepApproverModel> list);
        public Task<List<FormAuthViewModel>> FormAuthSetting(FormAuthSettingReqeustModel model);
        public Task<int> EditFormAuth(string uid, FormAuthEditModel model);
        public Task<bool> CheckAuthFormClassEditResult(string UID, string FormID);
    }
}
