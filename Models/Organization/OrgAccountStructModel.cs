using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Organization
{
    public class OrgAccountStructModel
    {
        public string UID { get; set; }
        public string DeptID { get; set; }
        public string DeptName { get; set; }
        public Int16 Main { get; set; }
        public int TitleID { get; set; }
        public string Title { get; set; }
        public Int16 SignApprover { get; set; }
        public string Agent { get; set; }
        public string AgentName { get; set; }
        public string CompanyType { get; set; }
        public string CompanyTypeName { get; set; }
        public Int16 Status { get; set; }
    }

    public class OrgAccountStructAddModel: OrgAccountStructModel
    {
        public bool isAdd { get; set; }
        public string oldDeptID { get; set; }
    }
}
