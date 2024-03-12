using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Organization
{
    public class OrgAccountInfoModel
    {
        public string UID { get; set; }
        public string GID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string EmployeeID { get; set; }
        public Int16 Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Editor { get; set; }
    }
}
