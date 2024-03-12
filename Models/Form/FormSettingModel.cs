using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Form
{
    public class FormSettingModel
    {
        public string FormClass { get; set; }
        public string FormName { get; set; }
        public int TypeID { get; set; }
        public string DefaultFlow { get; set; }
        public Int16 Status { get; set; }
        public int Sort { get; set; }
    }
}
