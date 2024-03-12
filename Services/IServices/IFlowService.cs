using FormsNet.Models.Flow;
using FormsNet.Models.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices
{
    public interface IFlowService
    {
        public Task<int> Start(string FormID);
        public Task<int> Sign(SignModel signModel);
        public Task<int> SignSetResult(string uid, SignFormModel signModel);
        public Task<FlowchartViewModel> GetFlowChart(FlowchartModel model);
    }
}
