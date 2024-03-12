using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Auth
{
    public class MenuAuthListGetModel
    {
        public string UID { get; set; }
        public string AppID { get; set; }
    }

    public class MenuAuthListModel
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public bool Status { get; set; }
        public int MainMenu { get; set; }
        public int Sort { get; set; }
    }

    public class MenuAuthSetModel
    {
        public string UID { get; set; }
        public string AppID { get; set; }
        public List<int> MenuList { get; set; }
        public List<int> MenuListAll { get; set; }
    }

}
