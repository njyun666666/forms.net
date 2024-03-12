using FormsNet.DB.IDB.Form;
using FormsNet.Models.Form;
using FormsNet.Models.Form.DEMO;
using FormsNet.Models.Sign;
using FormsNet.Services.IServices.Form;
using FormsNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.Form
{
    public class DEMOService : IDEMOService
    {
        IDB_Form_DEMO db_DEMO;

        public DEMOService(IDB_Form_DEMO dB_Form_DEMO)
        {
            db_DEMO = dB_Form_DEMO;
        }
        /// <summary>
        /// 取得表單資料
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<DEMOModel> GetFormData(string formID)
        {
            DEMOModel result = await db_DEMO.GetFormData(formID);
            return result;
        }
        /// <summary>
        /// 儲存草稿/申請表單 儲存表單資料
        /// </summary>
        /// <param name="listModel"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> Applicant(FormListModel listModel, DEMOModel model)
        {
            ResponseViewModel result = new ResponseViewModel();

            #region 正式送出申請 防呆
            if (model.Status == 1)
            {
                if (string.IsNullOrWhiteSpace(model.AAA))
                {
                    result.Code = (int)ResponseCode.fail;
                    result.Message = "請輸入 AAA欄位";
                }

            }

            if (result.Code == (int)ResponseCode.fail)
            {
                return result;
            }
            #endregion

            int dataSave = await db_DEMO.Applicant(listModel, model);

            if (dataSave == 0)
            {
                result.Code = (int)ResponseCode.fail;
                return result;
            }

            result.Code = 1;
            return result;
        }
        /// <summary>
        /// 簽核 儲存簽核log/表單資料
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="signModel"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> Sign(string uid, SignModel signModel, DEMOModel model)
        {
            ResponseViewModel result = new ResponseViewModel();

            #region 正式送出 防呆
            if (model != null && model.Status == 1)
            {
                // 依步驟
                if (signModel.StepID == 13)
                {
                    if (string.IsNullOrWhiteSpace(model.BBB))
                    {
                        result.Code = (int)ResponseCode.fail;
                        result.Message = "請輸入 BBB欄位";
                    }
                }

            }

            if (result.Code == (int)ResponseCode.fail)
            {
                return result;
            }
            #endregion


            int dataSave = await db_DEMO.Sign(uid, signModel, model);

            if (dataSave == 0)
            {
                result.Code = (int)ResponseCode.fail;
                return result;
            }

            result.Code = 1;
            return result;
        }

    }
}
