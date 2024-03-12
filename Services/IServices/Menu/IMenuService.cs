using FormsNet.DB.IDB;
using FormsNet.Models.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices.Menu
{
    public interface IMenuService
    {
        public Task<List<MenuItemModel>> App_Permission_Get(string GID);
        public Task<List<MenuModel>> Menu_Get();
        public Task<int> Menu_Add(MenuModel menuModel);
        public Task<int> Menu_Update(MenuModel menuModel);
        public List<MenuItemModel> MenuItem_find(int mainMenu);
        public Task<List<MenuTypeModel>> Menu_Type_Get();
        public Task<int> Menu_Type_Add(MenuTypeModel menuTypeModel);
        public Task<int> Menu_Type_Update(MenuTypeModel menuTypeModel);
    }
}
