using FormsNet.Models.Flow;
using FormsNet.Models.Sign;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB.IDB
{
    public interface IBackEndV2DB_Flow
    {
        public Task<string> GetFlowID(string formID);
        public Task<string> SetDefaultFlowID(string formID);
        public Task<string> GetDefaultFlowID(string formClass);
        public Task<int> AddFlowStepSystemLogError(string FormID, string Messagge);
        public Task<List<FlowchartStepModel>> FlowchartStep(string FlowID);
        public Task<List<FlowchartLineModel>> FlowchartLine(string FlowID);
        public Task<string> SetSerial(string FormID, string FlowID);
        public Task<int> Start(string FormID, string FlowID);
        public Task<int> Sign(SignModel signModel);
        public Task<int> SignSetResult(string uid, SignFormModel signModel);
        public Task<FlowStepModel> GetFlowStepByStepID(int StepID);
        public Task<FlowStepSystemLogModel> GetPrevFlowStepSystemNeedSign(MySqlConnection conn, MySqlTransaction transaction, Int64 NextSSID);
    }
}
