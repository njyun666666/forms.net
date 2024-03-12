using FormsNet.Models.Flow;
using FormsNet.Models.Sign;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB.IDB
{
    public interface IBackEndV2DB_Flow_Step
    {
        public Task<StepValueResponseModel> ApplicantSelf(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep);
        public Task<StepValueResponseModel> ApplicantDepartment(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep);
        public Task<StepValueResponseModel> DesignateList(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep);
        public Task<StepValueResponseModel> GotoSpecificDepartment(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep);
        public Task<StepValueResponseModel> GotoSpecificJobTitle(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep);
        public Task<StepValueResponseModel> Decision(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep);
        public Task<StepValueResponseModel> RouteSpecificDeptLevel(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep);
        public Task<StepValueResponseModel> GotoSpecificDeptLevel(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep);

    }
}
