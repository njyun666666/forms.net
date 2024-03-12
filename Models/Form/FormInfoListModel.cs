using FormsNet.Models.Sign;
using FormsNet.Services.IServices.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FormsNet.Models.Form
{
    public class FormInfoListModel
    {
        public int LevelID { get; set; }
        public int LevelNumber { get; set; }
        public string Level { get; set; }
        public string FormID { get; set; }
        public string FormName { get; set; }
        public string Serial { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantDept { get; set; }
        public DateTime ApplicantDate { get; set; }
        [JsonIgnore]
        public string NowStepApproverJSON { get; set; }
        public List<NowStepApproverViewModel> NowStepApproverList { get; set; }
        public int ResultID { get; set; }
        public string Result { get; set; }
    }
}
