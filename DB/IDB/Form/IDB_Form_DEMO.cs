using FormsNet.Models.Form;
using FormsNet.Models.Form.DEMO;
using FormsNet.Models.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB.IDB.Form
{
    public interface IDB_Form_DEMO
    {
        public Task<DEMOModel> GetFormData(string formID);
        public Task<int> Applicant(FormListModel listModel, DEMOModel model);
        public Task<int> Sign(string uid, SignModel signModel, DEMOModel model);
    }
}
