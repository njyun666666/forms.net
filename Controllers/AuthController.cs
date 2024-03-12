using FormsNet.Filters;
using FormsNet.Models.Auth;
using FormsNet.Models.Organization;
using FormsNet.Services.IServices;
using FormsNet.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormsNet.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : Controller
    {
        private ILogger<AuthController> _logger;
        private IAuthService _authService;


        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        
        [HttpPost]
        public async Task<IActionResult> Check([FromHeader] string uid, [FromBody] AuthCheckModel checkModel)
        {
            if(!await _authService.Check(uid, checkModel))
            {
                return Ok(new NoAuthResponse());
            }
            
            return Ok(new OKResponse());
        }
        /// <summary>
        /// 登入者是否為admin
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 14 })]
        [HttpPost]
        public async Task<IActionResult> IsAdmin([FromHeader] string uid)
        {
            return Ok(new OKResponse() { Data = _authService.IsAdmin(uid) });
        }
        /// <summary>
        /// 權限管理 取得使用者權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 14 })]
        [HttpPost]
        public async Task<IActionResult> GetUserMenuAuthList([FromHeader] string uid, [FromBody] MenuAuthListGetModel model)
        {
            return Ok(new OKResponse() { Data = await _authService.GetUserMenuAuthList(uid, model) });
        }
        /// <summary>
        /// 權限管理 修改使用者權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 14 })]
        [HttpPost]
        public async Task<IActionResult> SetUserMenuAuth([FromHeader] string uid, [FromBody] MenuAuthSetModel model)
        {
            return Ok(new ResponseViewModel(await _authService.SetUserMenuAuth(uid, model)));
        }


    }
}
