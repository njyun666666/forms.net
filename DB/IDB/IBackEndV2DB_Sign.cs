using FormsNet.Models.Flow;
using FormsNet.Models.Sign;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB.IDB
{
    public interface IBackEndV2DB_Sign
    {
        public Task<List<SignLogModel>> GetSignLogList(MySqlConnection conn, MySqlTransaction transaction, Int64 SSID);
        public Task<int> AddSignLog(MySqlConnection conn, MySqlTransaction transaction, string FormID, Int64? SSID, List<FlowApproverModel> List, FlowStepModel nowStep, SignFormModel signModel = null, Int16 SSIDFinish = 0);
        public Task<SignFormModel> GetSignForm(Int64 id);
        public Task<List<SignOptionModel>> GetSignOption(string uid, GetSignOptionModel model);
        public Task<int> Sign(MySqlConnection conn, MySqlTransaction transaction, string uid, SignModel signModel);
        public Task<int> SetStatus(Int64 SignID, int Status);
        public Task<List<SignLogViewModel>> SignLogList(string FormID);
        public Task<List<NowStepApproverModel>> NowStepApprover(string FormID);
    }
}
