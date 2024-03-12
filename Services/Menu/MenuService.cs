using FormsNet.DB.IDB;
using FormsNet.Models.Form;
using FormsNet.Models.Menu;
using FormsNet.Services.IServices.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.Menu
{
    public class MenuService : IMenuService
    {
        private IBackEndV2DB _backEndV2DB;

        List<MenuModel> MenuList;

        public MenuService(IBackEndV2DB backEndV2DB)
        {
            _backEndV2DB = backEndV2DB;
        }
        /// <summary>
        /// 取得使用者有權限的選單
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public async Task<List<MenuItemModel>> App_Permission_Get(string UID)
        {
            MenuList = await _backEndV2DB.PermissionQuery(UID);
            return MenuItem_find(0);
        }

        #region Menu
        public async Task<List<MenuModel>> Menu_Get()
        {
            return await _backEndV2DB.Menu_Query();
        }
        public async Task<int> Menu_Add(MenuModel menuModel)
        {
            return await _backEndV2DB.Menu_Add(menuModel);
        }
        public async Task<int> Menu_Update(MenuModel menuModel)
        {
            return await _backEndV2DB.Menu_Update(menuModel);
        }
        /// <summary>
        /// 組合選單
        /// </summary>
        /// <param name="mainMenu"></param>
        /// <returns></returns>
        public List<MenuItemModel> MenuItem_find(int mainMenu)
        {
            List<MenuItemModel> targetMenuList = MenuList.Where(x => x.MainMenu == mainMenu)
                .OrderBy(x => x.Sort)
                .Select(x => new MenuItemModel
                {
                    id = x.MenuID,
                    title = x.MenuName,
                    link = x.Url,
                    icon = x.Icon,
                }).ToList();


            targetMenuList.ForEach(x =>
            {
                List<MenuItemModel> tempList = MenuItem_find(x.id);

                if (tempList != null && tempList.Count > 0)
                {
                    x.children = new List<MenuItemModel>();
                    x.children.AddRange(tempList);
                }
            });


            return targetMenuList;
        }
        #endregion

        #region MenuType
        public async Task<List<MenuTypeModel>> Menu_Type_Get()
        {
            return await _backEndV2DB.Menu_Type_Query();
        }
        public async Task<int> Menu_Type_Add(MenuTypeModel menuTypeModel)
        {
            return await _backEndV2DB.Menu_Type_Add(menuTypeModel);
        }
        public async Task<int> Menu_Type_Update(MenuTypeModel menuTypeModel)
        {
            return await _backEndV2DB.Menu_Type_Update(menuTypeModel);
        }
        #endregion
    }
}
