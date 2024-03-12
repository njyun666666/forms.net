using FormsNet.Enums;
using FormsNet.Models.Form;
using FormsNet.Models.Search;
using FormsNet.Models.Sign;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB.IDB
{
    public interface IBackEndV2DB_Form
    {
        public Task<List<FormTypeModel>> GetFormTypeList(Int16 status = 1);
        public Task<List<FormSettingModel>> GetFormSettingList(Int16 status = 1);
        public Task<string> GetFormClass(string formID);
        public Task<FormSettingModel> GetFormSetting(string FormClass, string FormID);
        public Task<List<ApplicantListModel>> GetApplicantList(string uid);
        public Task<List<FormLevelModel>> GetFormLevel();
        public Task<int> DeleteDraft(string uid, string formID);
        public Task<int> Applicant(MySqlConnection conn, MySqlTransaction transaction, FormListModel model);
        public Task<int> SetResuult(MySqlConnection conn, MySqlTransaction transaction, string FormID, SignResultTypeEnum signResult);
        public Task<int> SetStatus(string FormID, int Status);
        public Task<FormListModel> GetFillerInfo(string uid);
        public Task<List<DraftListModel>> GetDraftList(string uid);
        public Task<bool> FormClassEnabled(string formClass);
        public Task<bool> FormAuth(string uid, string formID);
        public Task<bool> DraftAuth(string uid, string formID);
        public Task<bool> ApplicantAuth(string uid, string formID);
        public Task<bool> SignAuth(string uid, SignAuthModel model);
        public Task<bool> SignCancelAuth(string uid, SignAuthModel model);
        public Task<List<ApproverInfoModel>> GetApproverList(string uid);
        public Task<List<ApplicantInfoModel>> GetApplicantFormList(string uid);
        public Task<List<FormInfoListModel>> GetFormInfoList(string uid, SearchModel model);
        public Task<SignResultTypeModel> GetFormResult(string FormID);
        public Task<List<FormAuthModel>> GetFormAuthSetting(string UID);
        public Task<List<FormAuthTypeModel>> GetFormAuthType();
        public Task<int> EditFormAuth(string uid, FormAuthEditModel model);
        public Task<bool> CheckAuthFormClassEditResult(string UID, string FormID);
    }
}
