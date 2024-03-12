using FormsNet.Models.Search;
using FormsNet.Services.IServices.Form;
using FormsNet.Services.IServices.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        ISearchService _searchService;
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }
        [HttpPost]
        public async Task<IActionResult> FormInfoList([FromHeader] string UID, SearchModel model)
        {
            return Ok(await _searchService.FormInfoList(UID, model));
        }



    }
}
