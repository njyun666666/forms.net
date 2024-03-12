using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.ViewModel.Form
{
    public class FormsViewModel
    {
        public string TypeName { get; set; }
        public List<FormSettingViewModel> children { get; set; }
    }

    public class FormSettingViewModel
    {
        public string FormClass { get; set; }
        public string FormName { get; set; }
    }



}
