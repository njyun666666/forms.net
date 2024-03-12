using FormsNet.DB.IDB;
using FormsNet.Models.Flow;
using FormsNet.Models.Sign;
using FormsNet.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.Flow
{
    public class FlowService : IFlowService
    {
        IBackEndV2DB_Flow db_Flow;
        IBackEndV2DB_Form db_Form;
        IBackEndV2DB_Sign db_Sign;

        public FlowService(IBackEndV2DB_Flow backEndV2DB_Flow, IBackEndV2DB_Form backEndV2DB_Form, IBackEndV2DB_Sign backEndV2DB_Sign)
        {
            db_Flow = backEndV2DB_Flow;
            db_Form = backEndV2DB_Form;
            db_Sign = backEndV2DB_Sign;
        }
        /// <summary>
        /// 啟動流程
        /// </summary>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<int> Start(string FormID)
        {
            try
            {
                // 設定使用流程
                string FlowID = await db_Flow.GetFlowID(FormID);

                if (string.IsNullOrWhiteSpace(FlowID))
                {
                    FlowID = await db_Flow.SetDefaultFlowID(FormID);
                }

                if (string.IsNullOrWhiteSpace(FlowID))
                {
                    return -1;
                }

                // 設定表單編號
                await db_Flow.SetSerial(FormID, FlowID);


                return await db_Flow.Start(FormID, FlowID);


            }
            catch (Exception ex)
            {
                // 例外 表單設定草稿
                await db_Form.SetStatus(FormID, 0);

                return -1;
            }


        }
        /// <summary>
        /// 進行流程
        /// </summary>
        /// <param name="signModel"></param>
        /// <returns></returns>
        public async Task<int> Sign(SignModel signModel)
        {
            try
            {
                return await db_Flow.Sign(signModel);
            }
            catch (Exception ex)
            {
                // 例外 簽核設定草稿
                await db_Sign.SetStatus(signModel.ID, 0);

                _ = db_Flow.AddFlowStepSystemLogError(signModel.FormID, ex.Message);

                return -1;
            }
        }
        /// <summary>
        /// 修改表單狀態
        /// </summary>
        /// <param name="SignFormModel"></param>
        /// <returns></returns>
        public async Task<int> SignSetResult(string uid, SignFormModel signModel)
        {
            try
            {
                return await db_Flow.SignSetResult(uid, signModel);
            }
            catch (Exception ex)
            {
                _ = db_Flow.AddFlowStepSystemLogError(signModel.FormID, ex.Message);

                return -1;
            }
        }
        public async Task<FlowchartViewModel> GetFlowChart(FlowchartModel model)
        {
            FlowchartViewModel result = new FlowchartViewModel();


            if (string.IsNullOrWhiteSpace(model.flowID))
            {
                model.flowID = await db_Flow.GetDefaultFlowID(model.formClass);
            }

            Task getStep = Task.Run(async () =>
            {
                result.Step = await db_Flow.FlowchartStep(model.flowID);
            });

            Task getLine = Task.Run(async () =>
            {
                result.Line = await db_Flow.FlowchartLine(model.flowID);
            });

            await Task.WhenAll(getStep, getLine);

            return result;
        }



    }
}
