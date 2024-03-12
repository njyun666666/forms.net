using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Menu
{
    public class MenuTypeModel
    {
        public int TypeID { set; get; }
        public string TypeName { set; get; }
        public int Status { set; get; }
        public DateTime? CreateDate { set; get; }
        public DateTime? UpdateDate { set; get; }
        public string Editor { set; get; }
    }
}
