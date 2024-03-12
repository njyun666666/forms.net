using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Organization
{
    public class OrgAccountInfoCheckModel
    {
        public string UID { get; set; }
        public string Name { get; set; }
        public string EmployeeID { get; set; }
		public string PhotoURL { get; set; }
		public Int16 Status { get; set; }
        public string DeptID { get; set; }
        public Int16 S_Status { get; set; }
    }
}
