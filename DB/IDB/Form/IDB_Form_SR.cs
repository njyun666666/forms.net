using FormsNet.Models.Flow;
using FormsNet.Models.Form;
using FormsNet.Models.Form.SR;
using FormsNet.Models.Sign;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB.IDB
{
    public interface IDB_Form_SR 
    {
        public Task<SRModel> GetFormData(string formID);
        public Task<List<SRTaskOwnerModel>> GeTaskOwner(string formID);
        public Task<int> Applicant(FormListModel listModel, SRModel model);
        public Task<int> Sign(string uid, SignModel signModel, SRModel model);
        public Task<List<FlowApproverModel>> SRTaskOwner(MySqlConnection conn, MySqlTransaction transaction, string FormID);
    }
}
