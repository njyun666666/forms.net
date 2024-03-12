using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Organization
{
    public class OrgDeptInfoStructModel
    {
        public string DeptID { get; set; }
        public string DeptName { get; set; }
        public string ParentDept { get; set; }
        public Int16 Expand { get; set; }
        public int Sort { get; set; }
    }
    public class OrgDeptListModel
    {
        public string DeptID { get; set; }
        public string DeptName { get; set; }
        public string ParentDept { get; set; }
        public string ParentDeptName { get; set; }
        public int DeptLevelID { get; set; }
        public string LevelName { get; set; }
        public int Sort { get; set; }
        public Int16 Expand { get; set; }
        public Int16 Status { get; set; }
    }
    public class OrgDeptParentModel
    {
        public string DeptID { get; set; }
        public string ParentDept { get; set; }
        public int Level { get; set; }
    }

}
