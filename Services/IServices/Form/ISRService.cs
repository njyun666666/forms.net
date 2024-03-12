using FormsNet.Models.Form;
using FormsNet.Models.Form.SR;
using FormsNet.Models.Sign;
using FormsNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices.Form
{
    public interface ISRService
    {
        public Task<SRModel> GetFormData(string formID);
        public Task<ResponseViewModel> Applicant(FormListModel listModel, SRModel model);
        public Task<ResponseViewModel> Sign(string uid, SignModel signModel, SRModel model);
    }
}
