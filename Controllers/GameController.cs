using FormsNet.Filters;
using FormsNet.Models.Game;
using FormsNet.Services.IServices;
using FormsNet.Services.IServices.Game;
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
    public class GameController : ControllerBase
    {
        private ILogger<GameController> _logger;
        private IAuthService _authService;
        private IGameService _gameService;
        public GameController(ILogger<GameController> logger, IAuthService authService, IGameService gameService)
        {
            _logger = logger;
            _authService = authService;
            _gameService = gameService;
        }
        [HttpPost]
        public async Task<IActionResult> GameInfo_Get()
        {
            return Ok(new OKResponse { Data = await _gameService.GameInfo_Get() });
        }

        [TypeFilter(typeof(AuthFilter), Arguments = new object[] { 15 })]
        [HttpPost]
        public async Task<IActionResult> AddAppInfo([FromHeader] string uid, [FromBody] GameInfoAddModel model)
        {
            // 檢查參數
            if (string.IsNullOrWhiteSpace(model.AppID.Trim()))
            {
                return Ok(new FailResponse("請輸入 AppID"));
            }

            if (string.IsNullOrWhiteSpace(model.AppName.Trim()))
            {
                return Ok(new FailResponse("請輸入 遊戲名稱"));
            }

            if (string.IsNullOrWhiteSpace(model.Area.Trim()))
            {
                return Ok(new FailResponse("請輸入 地區"));
            }

            if (!model.TimeZone.HasValue)
            {
                return Ok(new FailResponse("請輸入 時區"));
            }

            return Ok(await _gameService.AddAppInfo(uid, model));
        }
        /// <summary>
        /// 依MenuID取得有權限的遊戲清單
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GameInfo_GetByAuth([FromHeader] string uid, [FromBody] GameInfoGetAuthModel model)
        {
            return Ok(new OKResponse() { Data = await _gameService.GameInfo_GetByAuth(model.MenuID, uid) });
        }


    }
}
