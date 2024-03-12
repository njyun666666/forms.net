using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Organization
{
    public class OrgAccountInfoStructAddModel
    {
        public string UID { get; set; }
        public string GID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string EmployeeID { get; set; }
        public Int16 Status { get; set; }
        public string Editor { get; set; }

        public string DeptID { get; set; }
        public Int16 Main { get; set; }
        public int TitleID { get; set; }
        public Int16 SignApprover { get; set; }
        public string CompanyType { get; set; }

    }
}
