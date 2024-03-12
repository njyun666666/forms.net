using FormsNet.Models.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Form
{
    public class ApplicantInfoModel
    {
        public int LevelID { get; set; }
        public int LevelNumber { get; set; }
        public string Level { get; set; }
        public string FormID { get; set; }
        public string FormName { get; set; }
        public string Serial { get; set; }
        public DateTime ApplicantDate { get; set; }
        public List<NowStepApproverViewModel> NowStepApproverList { get; set; }
    }
}
