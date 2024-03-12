using FormsNet.Models.Flow;
using FormsNet.Services.IServices;
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
    public class FlowController : ControllerBase
    {
        IFlowService _flowService;
        public FlowController(IFlowService flowService)
        {
            _flowService = flowService;
        }
        [HttpPost]
        public async Task<IActionResult> GetFlowchart(FlowchartModel model)
        {
            return Ok(await _flowService.GetFlowChart(model));
        }


    }
}
