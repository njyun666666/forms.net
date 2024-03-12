using FormsNet.Models.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.ViewModel.Organization
{
    public class OrgAccountInfoViewModel
    {
        public string UID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string EmployeeID { get; set; }
        public int Status { get; set; }
        public List<OrgAccountStructModel> StructList { get; set; }
    }
}
