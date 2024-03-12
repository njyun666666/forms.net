using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Sign
{
    public class NowStepApproverModel
    {
        public Int64 SSID { get; set; }
        public string StepName { get; set; }
        public string Approver { get; set; }
    }

    public class NowStepApproverViewModel
    {
        public string StepName { get; set; }
        public List<string> Approver { get; set; }
    }

}
