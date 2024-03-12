using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Organization
{
    public class OrgAccountInfoStructModel
    {
        public string UID { get; set; }
        public string Name { get; set; }
        public string DeptID { get; set; }
        public Int16 SignApprover { get; set; }
        public int Level { get; set; }
        public int Sort { get; set; }
    }
    public class OrgAccountListGetModel
    {
        public string EmployeeID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Int16? Status { get; set; }
    }
    public class OrgAccountListModel
    {
        public string UID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string EmployeeID { get; set; }
        public Int16 Status { get; set; }
        public int TitleID { get; set; }
        public string Title { get; set; }
        public Int16 SignApprover { get; set; }
        public string CompanyType { get; set; }
        public string CompanyTypeName { get; set; }
        public string DeptID { get; set; }
        public string DeptName { get; set; }
    }

}
