using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Form
{
    public class DraftListModel
    {
        public string FormID { get; set; }
        public string FormClass { get; set; }
        public string FormName { get; set; }
        public DateTime UpdateDate { get; set; }
    }
    public class DraftAuthModel
    {
        public string FormID { get; set; }
    }
}
