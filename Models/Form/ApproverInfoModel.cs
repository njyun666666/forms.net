using FormsNet.Models.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Form
{
    public class ApproverInfoModel
    {
        public Int64 ID { get; set; }
        public int LevelID { get; set; }
        public int LevelNumber { get; set; }
        public string Level { get; set; }
        public string FormID { get; set; }
        public string FormName { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string Serial { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantDept { get; set; }
        public List<NowStepApproverViewModel> NowStepApproverList { get; set; }
    }
    public class SignAuthModel
    {
        public Int64 ID { get; set; }
        public string FormID { get; set; }
    }
}
