using FormsNet.Filters;
using FormsNet.Models.Menu;
using FormsNet.Services.IServices;
using FormsNet.Services.IServices.Menu;
using FormsNet.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MenuController : Controller
    {
        private ILogger<MenuController> _logger;
        private IMenuService _menuService;
        private IAuthService _authService;
        public MenuController(ILogger<MenuController> logger, IMenuService menuService, IAuthService authService)
        {
            _logger = logger;
            _menuService = menuService;
            _authService = authService;
        }
        /// <summary>
        /// 取得使用者的側邊欄選單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> App_Permission_Get([FromHeader]string UID)
        {
            return Ok(new OKResponse { Data = await _menuService.App_Permission_Get(UID) });
        }

        #region Menu
        /// <summary>
        /// 取得選單
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 2 })]
        [HttpPost]
        public async Task<IActionResult> Menu_Get()
        {
            return Ok(new OKResponse { Data = await _menuService.Menu_Get() });
        }
        /// <summary>
        /// 新增選單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 2 })]
        [HttpPost]
        public async Task<IActionResult> Menu_Add([FromBody] MenuModel menuModel)
        {
            return Ok(new OKResponse { Data = await _menuService.Menu_Add(menuModel) });
        }
        /// <summary>
        /// 修改選單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 2 })]
        [HttpPost]
        public async Task<IActionResult> Menu_Update([FromBody] MenuModel menuModel)
        {
            return Ok(new OKResponse { Data = await _menuService.Menu_Update(menuModel) });
        }
        #endregion

        #region MenuType
        /// <summary>
        /// 取得選單/權限類型
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 5 })]
        [HttpPost]
        public async Task<IActionResult> Menu_Type_Get()
        {
            return Ok(new OKResponse { Data = await _menuService.Menu_Type_Get() });
        }
        /// <summary>
        /// 新增選單/權限類型
        /// </summary>
        /// <param name="menuTypeModel"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 5 })]
        [HttpPost]
        public async Task<IActionResult> Menu_Type_Add([FromBody] MenuTypeModel menuTypeModel)
        {
            return Ok(new OKResponse { Data = await _menuService.Menu_Type_Add(menuTypeModel) });
        }
        /// <summary>
        /// 修改選單/權限類型
        /// </summary>
        /// <param name="menuTypeModel"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 5 })]
        [HttpPost]
        public async Task<IActionResult> Menu_Type_Update([FromBody] MenuTypeModel menuTypeModel)
        {
            return Ok(new OKResponse { Data = await _menuService.Menu_Type_Update(menuTypeModel) });
        }
        #endregion
    }
}
