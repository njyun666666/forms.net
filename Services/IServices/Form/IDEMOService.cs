using FormsNet.Models.Form;
using FormsNet.Models.Form.DEMO;
using FormsNet.Models.Sign;
using FormsNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices.Form
{
    public interface IDEMOService
    {
        public Task<DEMOModel> GetFormData(string formID);
        public Task<ResponseViewModel> Applicant(FormListModel listModel, DEMOModel model);
        public Task<ResponseViewModel> Sign(string uid, SignModel signModel, DEMOModel model);
    }
}
