using FormsNet.DB.IDB;
using FormsNet.Models.Form;
using FormsNet.Models.Form.SR;
using FormsNet.Models.Sign;
using FormsNet.Services.IServices.Form;
using FormsNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.Form
{
    public class SRService : ISRService
    {
        IDB_Form_SR db_SR;

        public SRService(IDB_Form_SR dB_Form_SR)
        {
            db_SR = dB_Form_SR;
        }
        /// <summary>
        /// 取得表單資料
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<SRModel> GetFormData(string formID)
        {
            SRModel result = await db_SR.GetFormData(formID);
            result.taskOwnerList = await db_SR.GeTaskOwner(formID);

            return result;
        }
        /// <summary>
        /// 儲存草稿/申請表單 儲存表單資料
        /// </summary>
        /// <param name="listModel"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> Applicant(FormListModel listModel, SRModel model)
        {
            ResponseViewModel result = new ResponseViewModel();

            // 正式送出申請 防呆
            if (model.Status == 1)
            {

            }

            if (result.Code == (int)ResponseCode.fail)
            {
                return result;
            }

            int dataSave = await db_SR.Applicant(listModel, model);

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
        public async Task<ResponseViewModel> Sign(string uid, SignModel signModel, SRModel model)
        {
            ResponseViewModel result = new ResponseViewModel();

            // 正式送出 防呆
            if (model != null && model.Status == 1)
            {
                // 依步驟
                if (signModel.StepID == 4)
                {

                }


            }

            if (result.Code == (int)ResponseCode.fail)
            {
                return result;
            }

            int dataSave = await db_SR.Sign(uid, signModel, model);

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
