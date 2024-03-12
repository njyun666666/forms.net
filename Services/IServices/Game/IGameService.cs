using FormsNet.Models.Game;
using FormsNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices.Game
{
    public interface IGameService
    {
        public Task<List<GameInfoModel>> GameInfo_Get();
        public Task<ResponseViewModel> AddAppInfo(string editor, GameInfoAddModel model);
        public Task<List<GameInfoModel>> GameInfo_GetByAuth(int menuID, string uid);
    }
}
