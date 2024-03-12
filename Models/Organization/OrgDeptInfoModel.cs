using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Organization
{
    public class OrgDeptInfoModel
    {
        public string DeptID { get; set; }
        public string DeptName { get; set; }
        public Int16 Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Editor { get; set; }

    }
}
