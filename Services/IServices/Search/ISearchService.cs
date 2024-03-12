using FormsNet.Models.Form;
using FormsNet.Models.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices.Search
{
    public interface ISearchService
    {
        public Task<List<FormInfoListModel>> FormInfoList(string uid, SearchModel model);
    }
}
