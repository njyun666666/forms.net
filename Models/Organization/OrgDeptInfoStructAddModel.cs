using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Organization
{
    public class OrgDeptInfoStructAddModel
    {
        public bool IsAdd { get; set; }
        public string DeptID { get; set; }
        public string DeptName { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Editor { get; set; }
        public string ParentDept { get; set; }
        public int DeptLevelID { get; set; }
        public int Sort { get; set; }
    }
    public class DeptInfoStructEditModel
    {
        public string DeptID { get; set; }
        public string DeptName { get; set; }
        public int Status { get; set; }
        public string ParentDept { get; set; }
        public string ParentDeptName { get; set; }
        public int DeptLevelID { get; set; }
    }



}
