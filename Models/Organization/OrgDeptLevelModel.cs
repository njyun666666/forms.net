using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Organization
{
    public class OrgDeptLevelModel
    {
        public int DeptLevelID { get; set; }
        public string LevelName { get; set; }
        public int Level { get; set; }
        public Int16 Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Editor { get; set; }
    }
    public class OrgDeptLevelAddModel
    {
        public bool isAdd { get; set; }
        public int? DeptLevelID { get; set; }
        public string LevelName { get; set; }
        public int? Level { get; set; }
        public Int16 Status { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public class OrgDeptLevelGetModel
    {
        public Int16? Status { get; set; }
    }


}
