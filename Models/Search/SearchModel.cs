using FormsNet.Models.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Search
{
    public class SearchModel
    {
        public List<string> formClass { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string serial { get; set; }
        public List<OrgUserDeptModel> applicants { get; set; }
    }
}
