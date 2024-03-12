using FormsNet.DB.IDB;
using FormsNet.Enums;
using FormsNet.Models.Flow;
using FormsNet.Models.Form;
using FormsNet.Models.Form.DEMO;
using FormsNet.Models.Form.SR;
using FormsNet.Models.Search;
using FormsNet.Models.Sign;
using FormsNet.Services.IServices;
using FormsNet.Services.IServices.Form;
using FormsNet.ViewModel.Form;
using FormsNet.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormsNet.Services.Form
{
    public class FormService : IFormService
    {
        IBackEndV2DB_Form db_Form;
        IBackEndV2DB_Sign db_Sign;
        IBackEndV2DB_Flow db_Flow;
        IFlowService _flowService;

        #region form
        ISRService _srService;
        IDEMOService _demoService;
        #endregion

        public FormService(IBackEndV2DB_Form backEndV2DB_Form, IBackEndV2DB_Sign backEndV2DB_Sign, IFlowService flowService, IBackEndV2DB_Flow backEndV2DB_Flow,
            ISRService sRService,
            IDEMOService dEMOService
            )
        {
            db_Form = backEndV2DB_Form;
            db_Sign = backEndV2DB_Sign;
            db_Flow = backEndV2DB_Flow;
            _flowService = flowService;

            _srService = sRService;
            _demoService = dEMOService;
        }
        /// <summary>
        /// 取得啟用表單清單
        /// </summary>
        /// <returns></returns>
        public async Task<List<FormsViewModel>> GetForms(Int16 status = 1)
        {
            List<FormsViewModel> list = new List<FormsViewModel>();

            List<FormTypeModel> typelist = await db_Form.GetFormTypeList(status);
            List<FormSettingModel> formList = await db_Form.GetFormSettingList(status);


            typelist.ForEach(x =>
            {
                list.Add(new FormsViewModel()
                {
                    TypeName = x.TypeName,
                    children = formList.Where(a => a.TypeID == x.TypeID).Select(a => new FormSettingViewModel() { FormClass = a.FormClass, FormName = a.FormName }).ToList()
                });
            });

            return list;
        }
        public async Task<FormBaseDataModel> formBaseData(string uid, BaseDataRequestModel model)
        {
            FormBaseDataModel result = new FormBaseDataModel();
            FormSettingModel settingModel;



            Task task_getSetting = Task.Run(async () =>
            {
                settingModel = await db_Form.GetFormSetting(model.FormClass, model.FormID);
                result.FormClass = settingModel.FormClass;
                result.FormName = settingModel.FormName;
            });



            Task task_setNewApplicant = Task.Run(async () =>
            {
                if (model.StepType == (int)StepTypeEnum.NewApplicant)
                {

                    result.FormID = string.IsNullOrWhiteSpace(model.FormID) ? Guid.NewGuid().ToString() : model.FormID;
                    result.Applicant = new List<ApplicantModel>();
                    result.FileGroupID = string.IsNullOrWhiteSpace(model.FormID) ? Guid.NewGuid().ToString() : null;
                    result.StepType = (int)StepTypeEnum.NewApplicant;
                    result.StepTypeString = StepTypeEnum.NewApplicant.ToString();


                    // 取得申請人清單
                    List<ApplicantListModel> list = await db_Form.GetApplicantList(uid);

                    var uidList = list.Select(x => new { x.UID, x.Name }).Distinct().ToList();

                    uidList.ForEach(x =>
                    {
                        result.Applicant.Add(new ApplicantModel()
                        {
                            ApplicantID = x.UID,
                            ApplicantName = x.Name,
                            Depts = list.Where(d => d.UID == x.UID).Select(d => new ApplicantDeptModel() { ApplicantDeptID = d.DeptID, ApplicantDept = d.DeptName }).ToList()
                        });

                    });

                }


            });




            // 取得表單級別
            Task task_getLevels = Task.Run(async () =>
            {
                result.Levels = await db_Form.GetFormLevel();
            });


            
            Task task_getNowApprover = Task.Run(async () =>
            {
                if (!string.IsNullOrWhiteSpace(model.FormID))
                {
                    result.NowStepApproverList = await NowStepApprover(model.FormID);
                }
            });


            Task taskGetFormResult = Task.Run(async () =>
            {
                SignResultTypeModel signResult = await db_Form.GetFormResult(model.FormID);

                if (signResult != null)
                {
                    result.ResultID = signResult.ResultID;
                    result.Result = signResult.Result;
                }
            });


            // 等待全部完成
            await Task.WhenAll(
                task_getSetting,
                task_setNewApplicant,
                task_getLevels,
                task_getNowApprover,
                taskGetFormResult
                );

            return result;
        }
        /// <summary>
        /// 取得表單資料
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<object> GetFormData(string formID)
        {
            // 先取得FormClass
            string formClass = await db_Form.GetFormClass(formID);

            // 取得表單資料
            switch (formClass)
            {
                case "DEMO":
                    return await _demoService.GetFormData(formID);
                    
                case "SR":
                    return await _srService.GetFormData(formID);
                    
                default:
                    return null;
            }
        }
        /// <summary>
        /// 設定填單人資料
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        public async Task<FormListModel> SetFiller(string uid, FormListModel model)
        {
            FormListModel fillerModel = await db_Form.GetFillerInfo(uid);
            model.FillerID = fillerModel.FillerID;
            model.FillerName = fillerModel.FillerName;
            model.FillerDeptID = fillerModel.FillerDeptID;
            model.FillerDept = fillerModel.FillerDept;

            return model;
        }
        public async Task<int> DeleteDraft(string uid, string formID)
        {
            return await db_Form.DeleteDraft(uid, formID);
        }
        /// <summary>
        /// 儲存草稿/申請表單
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="jsonElement"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> Applicant(string uid, JsonElement jsonElement)
        {
            ResponseViewModel result = new ResponseViewModel();
            ResponseViewModel dataSave = new ResponseViewModel();

            FormListModel listModel = JsonSerializer.Deserialize<FormListModel>(jsonElement.GetRawText());
            string FormClass = listModel.FormClass;
            bool isDraft = listModel.Status == 0;



            // 先檢查表單狀態
            if (!await ApplicantAuth(uid, listModel.FormID))
            {
                result = new ResponseViewModel(ResponseCode.repeat_submit);
                return result;
            }



            listModel = await SetFiller(uid, listModel);


            // 正式送出申請
            if (!isDraft)
            {
                listModel.ApplicantDate = DateTime.Now;
            }

            // 儲存資料
            switch (FormClass)
            {
                case "DEMO":
                    DEMOModel demo_model = JsonSerializer.Deserialize<DEMOModel>(jsonElement.GetRawText());
                    dataSave = await _demoService.Applicant(listModel, demo_model);
                    break;
                case "SR":
                    SRModel sr_model = JsonSerializer.Deserialize<SRModel>(jsonElement.GetRawText());
                    dataSave = await _srService.Applicant(listModel, sr_model);
                    break;
                default:
                    break;
            }


            if (dataSave.Code != (int)ResponseCode.success)
            {
                return dataSave;
            }


            // 正式送出申請
            if (!isDraft)
            {
                // 開始流程
                int flowResult = await _flowService.Start(listModel.FormID);
                
                if (flowResult == 1)
                {
                    result = new ResponseViewModel(ResponseCode.success);
                }
                else
                {
                    result = new ResponseViewModel(ResponseCode.fail,"流程啟動失敗，已儲存草稿");
                }

            }
            else
            {
                // 存草稿成功
                result = new ResponseViewModel(ResponseCode.success);
            }


            return result;
        }
        public async Task<List<DraftListModel>> GetDraftList(string uid)
        {
            return await db_Form.GetDraftList(uid);
        }
        /// <summary>
        /// 表單是否啟用
        /// </summary>
        /// <param name="formClass"></param>
        /// <returns></returns>
        public async Task<bool> FormClassEnabled(string formClass)
        {
            return await db_Form.FormClassEnabled(formClass);
        }
        /// <summary>
        /// 表單查看權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<bool> FormAuth(string uid, string formID)
        {
            return await db_Form.FormAuth(uid, formID);
        }
        /// <summary>
        /// 草稿權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<bool> DraftAuth(string uid, string formID)
        {
            return await db_Form.DraftAuth(uid, formID);
        }
        /// <summary>
        /// 表單權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<bool> ApplicantAuth(string uid, string formID)
        {
            return await db_Form.ApplicantAuth(uid, formID);
        }
        /// <summary>
        /// 簽核權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="signID"></param>
        /// <returns></returns>
        public async Task<bool> SignAuth(string uid, SignAuthModel model)
        {
            return await db_Form.SignAuth(uid, model);
        }
        /// <summary>
        /// 撤回權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> SignCancelAuth(string uid, SignAuthModel model)
        {
            return await db_Form.SignCancelAuth(uid, model);
        }
        /// <summary>
        /// 取得待簽核清單
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<ApproverInfoModel>> GetApproverList(string uid)
        {
            List<ApproverInfoModel> list = await db_Form.GetApproverList(uid);

            foreach (var x in list)
            {
                x.NowStepApproverList = await NowStepApprover(x.FormID);
            }
            return list;
        }
        /// <summary>
        /// 取得進行中表單
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<ApplicantInfoModel>> GetApplicantFormList(string uid)
        {
            List<ApplicantInfoModel> list = await db_Form.GetApplicantFormList(uid);

            foreach (var x in list)
            {
                x.NowStepApproverList = await NowStepApprover(x.FormID);
            }
            return list;
        }
        /// <summary>
        /// 取得簽核表資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SignFormModel> GetSignForm(Int64 id)
        {
            SignFormModel result = await db_Sign.GetSignForm(id);
            FlowStepModel stepModel = await db_Flow.GetFlowStepByStepID(result.StepID);

            result.StepDescription = stepModel.Description;

            return result;
        }
        /// <summary>
        /// 取得簽核選項
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public async Task<List<SignOptionModel>> GetSignOption(string UID, GetSignOptionModel model)
        {
            return await db_Sign.GetSignOption(UID, model);
        }
        /// <summary>
        /// 簽核
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> Sign(string uid, SignModel model)
        {
            ResponseViewModel result = new ResponseViewModel();
            ResponseViewModel dataSave = new ResponseViewModel();


            // 先取得FormClass
            string FormClass = await db_Form.GetFormClass(model.FormID);

            bool isDraft = model.Status == 0;



            // 正式送出申請
            if (!isDraft)
            {
                //listModel.ApplicantDate = DateTime.Now;
            }

            // 儲存資料
            switch (FormClass)
            {
                case "DEMO":
                    DEMOModel demo_model = JsonSerializer.Deserialize<DEMOModel>(model.FormData.GetRawText());
                    dataSave = await _demoService.Sign(uid, model, demo_model);
                    break;
                case "SR":
                    SRModel sr_model = JsonSerializer.Deserialize<SRModel>(model.FormData.GetRawText());
                    dataSave = await _srService.Sign(uid, model, sr_model);
                    break;
                default:
                    break;
            }


            if (dataSave.Code != (int)ResponseCode.success)
            {
                return dataSave;
            }


            // 正式送出簽核
            if (!isDraft)
            {
                // 開始流程
                int flowResult = await _flowService.Sign(model);

                if (flowResult == 1)
                {
                    result = new ResponseViewModel(ResponseCode.success);
                }
                else
                {
                    result = new ResponseViewModel(ResponseCode.fail, "流程失敗，已儲存草稿");
                }

            }
            else
            {
                // 存草稿成功
                result = new ResponseViewModel(ResponseCode.success);
            }


            return result;
        }
        /// <summary>
        /// 修改表單狀態
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> SignSetResult(string uid, SignFormModel model)
        {
            ResponseViewModel result = new ResponseViewModel();

            int flowResult = await _flowService.SignSetResult(uid, model);

            if (flowResult == 1)
            {
                result = new ResponseViewModel(ResponseCode.success);
            }
            else
            {
                result = new ResponseViewModel(ResponseCode.fail, "流程失敗");
            }

            return result;
        }
        /// <summary>
        /// 取得簽核紀錄
        /// </summary>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<List<SignLogViewModel>> SignLogList(string FormID)
        {
            return await db_Sign.SignLogList(FormID);
        }
        /// <summary>
        /// 取得目前步驟/簽核人
        /// </summary>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<List<NowStepApproverViewModel>> NowStepApprover(string FormID)
        {
            List<NowStepApproverModel> list = await db_Sign.NowStepApprover(FormID);
            return SetNowStepApprover(list);
        }
        /// <summary>
        /// 取得目前步驟/簽核人 from json string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public List<NowStepApproverViewModel> NowStepApprover_json(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            List<NowStepApproverModel> list = JsonSerializer.Deserialize<List<NowStepApproverModel>>(json);
            return SetNowStepApprover(list);
        }
        /// <summary>
        /// 轉成指定model 目前步驟/簽核人
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<NowStepApproverViewModel> SetNowStepApprover(List<NowStepApproverModel> list)
        {
            List<NowStepApproverViewModel> result = new List<NowStepApproverViewModel>();

            var ssid = list.Select(x => x.SSID).Distinct().ToList();

            ssid.ForEach(x =>
            {
                var temp = list.Where(a => a.SSID == x);

                string stepName = temp.FirstOrDefault().StepName;
                List<string> approvers = temp.Select(a => a.Approver).ToList();

                result.Add(new NowStepApproverViewModel() { StepName = stepName, Approver = approvers });
            });

            return result;
        }

        #region FormAuth Setting
        /// <summary>
        /// 取得使用者表單權限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<List<FormAuthViewModel>> FormAuthSetting(FormAuthSettingReqeustModel model)
        {
            List<FormAuthViewModel> result = new List<FormAuthViewModel>();

            List<FormsViewModel> formList = null;
            List<FormAuthModel> authSettingList = null;
            List<FormAuthTypeModel> authTypeList = null;

            Task taskFormList = Task.Run(async () =>
            {
                formList = await GetForms();
            });

            Task taskAuthSettingList = Task.Run(async () =>
            {
                authSettingList = await db_Form.GetFormAuthSetting(model.UID);
            });

            Task taskAuthTypeList = Task.Run(async () =>
            {
                authTypeList = await db_Form.GetFormAuthType();
            });

            await Task.WhenAll(taskFormList, taskAuthSettingList, taskAuthTypeList);


            formList.ForEach(type =>
            {
                List<FormSettingAuthViewModel> forms = new List<FormSettingAuthViewModel>();

                type.children.ForEach(form =>
                {
                    List<FormAuthStatusModel> statusList = authTypeList.Select(x => new FormAuthStatusModel
                    {
                        AuthType = x.AuthType,
                        TypeName = x.TypeName,
                        Status = authSettingList.Where(s => s.FormClass == form.FormClass && s.AuthType == x.AuthType).Any() ? 1 : 0
                    }).ToList();

                    forms.Add(new FormSettingAuthViewModel()
                    {
                        FormClass = form.FormClass,
                        FormName = form.FormName,
                        Auth = statusList
                    });
                });

                result.Add(new FormAuthViewModel()
                {
                    TypeName = type.TypeName,
                    children = forms
                });
            });


            return result;
        }
        public async Task<int> EditFormAuth(string uid, FormAuthEditModel model)
        {
            return await db_Form.EditFormAuth(uid, model);
        }
        public async Task<bool> CheckAuthFormClassEditResult(string UID, string FormID)
        {
            return await db_Form.CheckAuthFormClassEditResult(UID, FormID);
        }

        #endregion
    }
}
