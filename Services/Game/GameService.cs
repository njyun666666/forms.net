using FormsNet.DB.IDB;
using FormsNet.Models.Game;
using FormsNet.Services.IServices.Game;
using FormsNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.Game
{
    public class GameService: IGameService
    {
        private IBackEndV2DB _backEndV2DB;
        public GameService(IBackEndV2DB backEndV2DB)
        {
            _backEndV2DB = backEndV2DB;
        }
        public async Task<List<GameInfoModel>> GameInfo_Get()
        {
            return await _backEndV2DB.GameInfoQuery();
        }
        public async Task<ResponseViewModel> AddAppInfo(string editor, GameInfoAddModel model)
        {
            return await _backEndV2DB.AddAppInfo(editor, model);
        }
        /// <summary>
        /// 取得分遊戲權限的遊戲清單
        /// </summary>
        /// <param name="menuID"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<GameInfoModel>> GameInfo_GetByAuth(int menuID, string uid)
        {
            return await _backEndV2DB.GameInfo_GetByAuth(menuID, uid);
        }

    }
}
