using FormsNet.DB.IDB;
using FormsNet.Models.Form;
using FormsNet.Models.Search;
using FormsNet.Services.IServices.Form;
using FormsNet.Services.IServices.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.Search
{
    public class SearchService : ISearchService
    {
        IBackEndV2DB_Form db_Form;
        IFormService _formService;

        public SearchService(IBackEndV2DB_Form backEndV2DB_Form, IFormService formService)
        {
            db_Form = backEndV2DB_Form;
            _formService = formService;
        }

        public async Task<List<FormInfoListModel>> FormInfoList(string uid, SearchModel model)
        {
            List<FormInfoListModel> list = await db_Form.GetFormInfoList(uid, model);

            list.ForEach(x =>
            {
                x.NowStepApproverList = _formService.NowStepApprover_json(x.NowStepApproverJSON);
            });

            return list;
        }

    }
}
