using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Menu
{
    public class MenuModel
    {
        public int MenuID { set; get; }
        public string MenuName { set; get; }
        public int MainMenu { set; get; }
        public string Url { set; get; }
        public string Icon { set; get; }
        public int Status { set; get; }
        public int Type { set; get; }
        public int Sort { set; get; }
        public DateTime? CreateDate { set; get; }
        public DateTime? UpdateDate { set; get; }
        public string Editor { set; get; }
    }
}
