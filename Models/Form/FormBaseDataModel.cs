using FormsNet.Enums;
using FormsNet.Models.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Form
{
    public class FormBaseDataModel
    {
        public FormBaseDataModel()
        {
        }
        public string FormClass { get; set; }
        public string FormName { get; set; }
        public string FormID { get; set; }
        public Int64? SignID { get; set; }
        public int? StepID { get; set; }
        public int? StepType { get; set; }
        public string StepTypeString { get; set; }
        public List<ApplicantModel> Applicant { get; set; }
        public List<FormLevelModel> Levels { get; set; }
        public string FileGroupID { get; set; }
        public List<NowStepApproverViewModel> NowStepApproverList { get; set; }
        public SignResultTypeEnum ResultID { get; set; }
        public string Result { get; set; }
    }
    public class ApplicantModel
    {
        public string ApplicantID { get; set; }
        public string ApplicantName { get; set; }
        public List<ApplicantDeptModel> Depts { get; set; }
    }
    public class ApplicantDeptModel
    {
        public string ApplicantDeptID { get; set; }
        public string ApplicantDept { get; set; }
    }

    public class ApplicantListModel
    {
        public string UID { get; set; }
        public string Name { get; set; }
        public string DeptID { get; set; }
        public string DeptName { get; set; }
    }


    public class BaseDataRequestModel
    {
        public string FormClass { get; set; }
        public string FormID { get; set; }
        public int? StepType { get; set; }
        public Int64? SignID { get; set; }
    }

}
