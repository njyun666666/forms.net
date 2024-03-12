using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Organization
{
    public class OrgPickerDeptInfoModel
    {
        public string DeptID { get; set; }
        public string DeptName { get; set; }
    }
    public class OrgPickerUserInfoModel
    {
        public string UID { get; set; }
        public string Name { get; set; }
        public string EmployeeID { get; set; }
        public string DeptID { get; set; }
        public string DeptName { get; set; }
        public Int16 Main { get; set; }
    }

    public class OrgPickerAccountInfoGetModel
    {
        public bool onlyDeptUser { get; set; }
        public List<string> whichDeptUser { get; set; }
    }
}
