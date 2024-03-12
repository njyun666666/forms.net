using FormsNet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Flow
{
    public class StepValueResponseModel
    {
        public bool StepAllComplete { get; set; }
        public SignResultTypeEnum SignResult { get; set; }
        public string Value { get; set; }
        public List<FlowApproverModel> List { get; set; }
        public StepValueResponseModel()
        {
            StepAllComplete = false;
            List = new List<FlowApproverModel>();
        }
    }
}
