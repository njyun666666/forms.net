using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Organization
{
    public class OrgTitleModel
    {
        public int TitleID { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public Int16 Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Editor { get; set; }
    }
    public class OrgTitleAddModel
    {
        public bool isAdd { get; set; }
        public int? TitleID { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public Int16 Status { get; set; }
    }

    public class OrgTitleGetModel
    {
        public Int16? Status { get; set; }
    }

}
