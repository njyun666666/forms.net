using FormsNet.DB.DBClass;
using FormsNet.DB.IDB;
using FormsNet.Enums;
using FormsNet.Models.Auth;
using FormsNet.Models.Form;
using FormsNet.Models.Game;
using FormsNet.Models.Login;
using FormsNet.Models.Menu;
using FormsNet.Models.Organization;
using FormsNet.ViewModel.Auth;
using FormsNet.ViewModels;
using Dapper;

using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB
{
    public class BackEndV2DB : IBackEndV2DB
    {
        public readonly string DbName = "BackEndV2DB";
        public string str_conn;

        public IDBConnection _dBConnection;

        public BackEndV2DB(IDBConnection dBConnection)
        {
            _dBConnection = dBConnection;
            str_conn = _dBConnection.Connection(DbName);
        }

		#region login
		/// <summary>
		/// 新增登入Log
		/// </summary>
		/// <param name="authToken"></param>
		/// <param name="ip"></param>
		public async Task<int> AddLoginLog(AuthTokenViewModel authToken, string ip)
        {
            string sql = "INSERT INTO `TB_Login_Log`"+
                        " (`TokenKey`,`UID`,`Date`,`IP`,`Status`) "+
                        " VALUES (@in_TokenKey, @in_UID, now(), @in_IP, 1); ";
            

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_TokenKey", authToken.TokenKey, DbType.String);
            _params.Add("@in_UID", authToken.UID, DbType.String);
            _params.Add("@in_IP", ip, DbType.String);

            return await AsyncDB.ExecuteAsync(str_conn, sql, _params, null, false);
        }
        /// <summary>
        /// 檢查TokenKey
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> CheckTokenKey(AuthTokenViewModel model)
		{
            string sql = "select exists(" +
                        "	select 1 from TB_Login_Log where TokenKey=@in_TokenKey and UID=@in_UID and Status=1" +
                        ")";
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_TokenKey", model.TokenKey, DbType.String);
            _params.Add("@in_UID", model.UID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<bool>(str_conn, sql, _params);
        }
        #endregion

        #region 選單
        /// <summary>
        /// 取得使用者有權限的選單
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public async Task<List<MenuModel>> PermissionQuery(string UID)
        {
            string sql = "select distinct m.* from TB_Menu m left" +
                        " join TB_App_Permission p on p.MenuID = m.MenuID" +
                        " where m.Type=1 and m.Status = 1 and ( (m.AuthType = 1) or (p.UID = @in_UID and p.Status = 1) )";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_UID", UID, DbType.String, size: 255);

            return await AsyncDB.QueryAsync<MenuModel>(str_conn, sql, _params);
        }
        public async Task<List<MenuModel>> Menu_Query()
        {
            string sql = $"select * from  TB_Menu order by Sort";
            return await AsyncDB.QueryAsync<MenuModel>(str_conn, sql);
        }
        public async Task<int> Menu_Add(MenuModel menuModel)
        {
            string sql = $"insert into TB_Menu(MenuName,MainMenu,Url,Icon,Sort,Status,Type,CreateDate)  " +
                          " values (@MenuName,@MainMenu,@Url,@Icon,@Sort,@Status,@Type,now())";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@MenuName", menuModel.MenuName, DbType.String, size: 255);
            _params.Add("@MainMenu", menuModel.MainMenu, DbType.Int64);
            _params.Add("@Url", menuModel.Url, DbType.String, size: 255);
            _params.Add("@Icon", menuModel.Icon, DbType.String, size: 255);
            _params.Add("@Status", menuModel.Status, DbType.Int32);
            _params.Add("@Type", menuModel.Type, DbType.Int32);
            _params.Add("@Sort", menuModel.Sort, DbType.Int64);
            
            return await AsyncDB.ExecuteAsync(str_conn, sql, _params);
        }
        public async Task<int> Menu_Update(MenuModel menuModel)
        {
            string sql = "update TB_Menu set " +
                         " MenuName = @MenuName , MainMenu = @MainMenu , Url = @Url," +
                         " Icon = @Icon, Status = @Status , Type = @Type , Sort = @Sort , UpdateDate = now()" +
                         " where MenuID = @MenuID";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@MenuID", menuModel.MenuID, DbType.Int64);
            _params.Add("@MenuName", menuModel.MenuName, DbType.String, size: 255);
            _params.Add("@MainMenu", menuModel.MainMenu, DbType.Int64);
            _params.Add("@Url", menuModel.Url, DbType.String, size: 255);
            _params.Add("@Icon", menuModel.Icon, DbType.String, size: 255);
            _params.Add("@Status", menuModel.Status, DbType.Int32);
            _params.Add("@Type", menuModel.Type, DbType.Int32);
            _params.Add("@Sort", menuModel.Sort, DbType.Int64);

            return await AsyncDB.ExecuteAsync(str_conn, sql, _params);
        }

        public async Task<List<MenuTypeModel>> Menu_Type_Query()
        {
            string sql = $"select * from  TB_Menu_Type";
            return await AsyncDB.QueryAsync<MenuTypeModel>(str_conn, sql);
        }
        public async Task<int> Menu_Type_Add(MenuTypeModel menuTypeModel)
        {
            string sql = $"insert into TB_Menu_Type(TypeName,Status,CreateDate) values " +
                          " (@TypeName,@Status,now())";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@TypeName", menuTypeModel.TypeName, DbType.String, size: 255);
            _params.Add("@Status", menuTypeModel.Status, DbType.Int32);

            return await AsyncDB.ExecuteAsync(str_conn, sql, _params);
        }
        public async Task<int> Menu_Type_Update(MenuTypeModel menuTypeModel)
        {
            string sql = "update TB_Menu_Type set " +
                        " TypeName = @TypeName , Status = @Status , UpdateDate = now() " +
                        " where TypeID = @TypeID";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@TypeID", menuTypeModel.TypeID, DbType.Int64);
            _params.Add("@TypeName", menuTypeModel.TypeName, DbType.String, size: 255);
            _params.Add("@Status", menuTypeModel.Status, DbType.Int32);

            return await AsyncDB.ExecuteAsync(str_conn, sql, _params);
        }
        #endregion

        #region 遊戲
        /// <summary>
        /// 取得遊戲清單
        /// </summary>
        /// <returns></returns>
        public async Task<List<GameInfoModel>> GameInfoQuery()
        {
            string sql = "select * from TB_App_Info";
            return await AsyncDB.QueryAsync<GameInfoModel>(str_conn, sql);
        }
        /// <summary>
        /// 取得分遊戲權限的遊戲清單
        /// </summary>
        /// <param name="menuID"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<GameInfoModel>> GameInfo_GetByAuth(int menuID, string uid)
        {
            string sql = "select a.* from TB_App_Info a join TB_App_Permission p on a.AppID=p.AppID" +
                         " where p.MenuID = @in_MenuID and p.UID = @in_UID and p.Status = 1";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_UID", uid, DbType.String, size: 255);
            _params.Add("@in_MenuID", menuID, DbType.Int32);

            return await AsyncDB.QueryAsync<GameInfoModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 新增/修改遊戲資訊
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> AddAppInfo(string editor, GameInfoAddModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 判斷AppID重覆
                        if (model.isAdd)
                        {
                            string sql_appid = "select exists(select 1 from TB_App_Info where UPPER(AppID) = UPPER(@in_AppID) )";

                            DynamicParameters _params_appid = new DynamicParameters();
                            _params_appid.Add("@in_AppID", model.AppID.Trim(), DbType.String, size: 255);


                            bool title_isExists = await conn.QueryFirstOrDefaultAsync<bool>(sql_appid, _params_appid, transaction);

                            if (title_isExists)
                            {
                                return new ResponseViewModel(ResponseCode.fail, "AppID重覆");
                            }

                            model.Status = 1;

                            string sql = "INSERT INTO TB_App_Info (`AppID`,`AppName`,`Status`,`Area`,`TimeZone`,`CreateDate`,`UpdateDate`,`Editor`)" +
                                        " VALUES(@in_AppID, @in_AppName, @in_Status, @in_Area, @in_TimeZone, now(), now(), @in_Editor); ";

                            DynamicParameters _params = new DynamicParameters();
                            _params.Add("@in_AppID", model.AppID.Trim(), DbType.String, size: 255);
                            _params.Add("@in_AppName", model.AppName.Trim(), DbType.String, size: 255);
                            _params.Add("@in_Area", model.Area.Trim(), DbType.String, size: 255);
                            _params.Add("@in_TimeZone", model.TimeZone, DbType.String, size: 255);
                            _params.Add("@in_Status", model.Status, DbType.Int16);
                            _params.Add("@in_Editor", editor, DbType.String, size: 255);

                            await conn.ExecuteAsync(sql, _params, transaction);
                        }
                        else
                        {
                            string sql = "UPDATE TB_App_Info" +
                                        " SET" +
                                        " AppName = @in_AppName," +
                                        //" Status = @in_Status," +
                                        " Area = @in_Area," +
                                        " TimeZone = @in_TimeZone," +
                                        " UpdateDate = now()," +
                                        " Editor = @in_Editor" +
                                        " WHERE AppID = @in_AppID;";

                            DynamicParameters _params = new DynamicParameters();
                            _params.Add("@in_AppID", model.AppID.Trim(), DbType.String, size: 255);
                            _params.Add("@in_AppName", model.AppName.Trim(), DbType.String, size: 255);
                            _params.Add("@in_Area", model.Area.Trim(), DbType.String, size: 255);
                            _params.Add("@in_TimeZone", model.TimeZone, DbType.String, size: 255);
                            //_params.Add("@in_Status", model.Status, DbType.Int16);
                            _params.Add("@in_Editor", editor, DbType.String, size: 255);

                            await conn.ExecuteAsync(sql, _params, transaction);
                        }


                        await transaction.CommitAsync();
                        return new ResponseViewModel(ResponseCode.success);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await transaction.RollbackAsync();
                        //throw ex;
                        return new ResponseViewModel(ResponseCode.fail, ex.Message);
                    }
                    finally
                    {
                        //if (conn.State != ConnectionState.Closed) conn.Close();
                        //conn.Dispose();
                    }
                }
            }

        }

        #endregion

        #region 組織
        public async Task<List<OrgAccountInfoCheckModel>> CheckAccountPassword(LoginModel model)
		{
            string sql = "select i.UID, i.Name, i.EmployeeID, i.PhotoURL, i.Status, s.DeptID, s.Status as S_Status" +
                    " from TB_Org_Account_Info i left join TB_Org_Account_Struct s on i.UID=s.UID" +
                    " where i.Account=@in_Account and i.Password=@in_Password ";
            
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_Account", model.Account, DbType.String);
            _params.Add("@in_Password", model.Password, DbType.String);

            return await AsyncDB.QueryAsync<OrgAccountInfoCheckModel>(str_conn, sql, _params);
        }
        public async Task<List<OrgAccountInfoCheckModel>> OrgAccountInfoByUID(string uid)
        {
            string sql = "select i.UID, i.Name, i.EmployeeID, i.PhotoURL, i.Status, s.DeptID, s.Status as S_Status" +
                    " from TB_Org_Account_Info i left join TB_Org_Account_Struct s on i.UID=s.UID" +
                    " where i.UID=@in_uid ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String);

            return await AsyncDB.QueryAsync<OrgAccountInfoCheckModel>(str_conn, sql, _params);
        }
        public async Task<string> GetEmailByAccount(string account)
		{
            string sql = "select Email from TB_Org_Account_Info where Account=@in_account ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_account", account, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<string>(str_conn, sql, _params);
        }
        /// <summary>
        /// 依Email 取得帳號資訊
        /// </summary>
        /// <param name="GID"></param>
        /// <returns></returns>
        public List<OrgAccountInfoCheckModel> GetAccountInfoByEmail(string email)
        {
            string sql = "select i.UID, i.Name, i.EmployeeID, i.Status, s.DeptID, s.Status as S_Status " +
                         " from TB_Org_Account_Info i left join TB_Org_Account_Struct s on i.UID=s.UID" +
                         " where i.Email = @in_email ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_email", email, DbType.String, size: 255);

            return SystemDB.Query<OrgAccountInfoCheckModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得部門層級
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<List<OrgDeptLevelModel>> GetDeptLevel(OrgDeptLevelGetModel model)
        {
            DynamicParameters _params = new DynamicParameters();

            string sql = "select * from TB_Org_Dept_Level where 1=1";
            
            if (model.Status.HasValue)
            {
                sql += " and Status=@in_status";
                _params.Add("@in_status", model.Status, DbType.Int16);
            }

            sql += " order by Level desc ";

            return await AsyncDB.QueryAsync<OrgDeptLevelModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 新增/修改部門
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> AddDeptInfoStruct(string editor, OrgDeptInfoStructAddModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 檢查名稱重覆
                        string sql = "select exists(select 1 from TB_Org_Dept_Info where UPPER(DeptName) = UPPER(@in_DeptName))";
                        
                        if (!model.IsAdd)
                        {
                            sql = "select exists(select 1 from TB_Org_Dept_Info where UPPER(DeptName) = UPPER(@in_DeptName) and DeptID!=@in_DeptID )";
                        }

                        DynamicParameters _params = new DynamicParameters();
                        _params.Add("@in_DeptID", model.DeptID, DbType.String, size: 255);
                        _params.Add("@in_DeptName", model.DeptName.Trim(), DbType.String, size: 255);

                        bool isExists = await conn.QueryFirstOrDefaultAsync<bool>(sql, _params, transaction);

                        if (isExists)
                        {
                            return new ResponseViewModel(ResponseCode.fail, "部門名稱重覆");
                        }

                        // 檢查迴圈
                        DynamicParameters _params_child = new DynamicParameters();
                        _params_child.Add("@in_DeptID", model.DeptID, DbType.String, size: 255);

                        List<OrgGetChildDeptModel> childList = (await conn.QueryAsync<OrgGetChildDeptModel>("SP_Org_GetChildDept", _params_child, transaction, commandType: CommandType.StoredProcedure)).ToList();

                        if (childList != null && childList.Count > 0)
                        {
                            if (childList.Where(x => x.DeptID == model.ParentDept).Any())
                            {
                                return new ResponseViewModel(ResponseCode.fail, "不能選此部門/子部門為上層部門");
                            }
                        }



                        // 新增部門
                        if (model.IsAdd)
                        {
                            // 取得上層部門排序最後一筆數字
                            DynamicParameters _params_sort = new DynamicParameters();

                            string sql_sort = "select Sort from TB_Org_Dept_Struct where ";

                            if (string.IsNullOrWhiteSpace(model.ParentDept))
                            {
                                sql_sort += " ParentDept is null";
                            }
                            else
                            {
                                sql_sort += " ParentDept = @in_ParentDept";
                                _params_sort.Add("@in_ParentDept", model.ParentDept, DbType.String, size: 255);
                            }

                            sql_sort += " order by Sort desc limit 1";
                            int maxSort = await conn.QueryFirstOrDefaultAsync<int>(sql_sort, _params_sort, transaction);




                            // insert
                            string deptID = Guid.NewGuid().ToString();

                            string sql_insert = "insert into TB_Org_Dept_Info (DeptID, DeptName, Status, CreateDate, Editor)" +
                                         " values(@in_DeptID, @in_DeptName, @in_Status, now(), @in_UID); " +

                                         "insert into TB_Org_Dept_Struct (DeptID, ParentDept, DeptLevelID, Status, CreateDate, Editor, Sort)" +
                                        " values(@in_DeptID, @in_ParentDept, @in_DeptLevelID, @in_Status, now(), @in_UID, @in_Sort ); ";

                            DynamicParameters _params_insert = new DynamicParameters();
                            _params_insert.Add("@in_UID", editor, DbType.String, size: 255);
                            _params_insert.Add("@in_DeptID", deptID, DbType.String, size: 255);
                            _params_insert.Add("@in_DeptName", model.DeptName.Trim(), DbType.String, size: 255);
                            _params_insert.Add("@in_Status", model.Status, DbType.Int16);
                            _params_insert.Add("@in_ParentDept", model.ParentDept, DbType.String, size: 255);
                            _params_insert.Add("@in_DeptLevelID", model.DeptLevelID, DbType.Int32);
                            _params_insert.Add("@in_Sort", maxSort + 1, DbType.Int32);


                            await conn.ExecuteAsync(sql_insert, _params_insert, transaction);

                        }
                        else
                        {
                            // 修改
                            string sql_update = " UPDATE TB_Org_Dept_Info" +
                                                " SET" +
                                                " DeptName = @in_DeptName," +
                                                " Status = @in_Status," +
                                                " UpdateDate = now()," +
                                                " Editor = @in_UID" +
                                                " WHERE DeptID = @in_DeptID ; " +

                                                " UPDATE TB_Org_Dept_Struct" +
                                                " SET" +
                                                " ParentDept = @in_ParentDept," +
                                                " DeptLevelID = @in_DeptLevelID," +
                                                " Status = @in_Status," +
                                                " UpdateDate = now()," +
                                                " Editor = @in_UID" +
                                                " WHERE DeptID = @in_DeptID;";


                            DynamicParameters _params_update = new DynamicParameters();
                            _params_update.Add("@in_UID", editor, DbType.String, size: 255);
                            _params_update.Add("@in_DeptID", model.DeptID, DbType.String, size: 255);
                            _params_update.Add("@in_DeptName", model.DeptName.Trim(), DbType.String, size: 255);
                            _params_update.Add("@in_Status", model.Status, DbType.Int16);
                            _params_update.Add("@in_ParentDept", model.ParentDept, DbType.String, size: 255);
                            _params_update.Add("@in_DeptLevelID", model.DeptLevelID, DbType.Int32);


                            await conn.ExecuteAsync(sql_update, _params_update, transaction);

                        }



                        await transaction.CommitAsync();
                        return new ResponseViewModel(ResponseCode.success);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await transaction.RollbackAsync();
                        //throw ex;
                        return new ResponseViewModel(ResponseCode.fail, ex.Message);
                    }
                    finally
                    {
                        //if (conn.State != ConnectionState.Closed) conn.Close();
                        //conn.Dispose();
                    }
                }
            }

            return new ResponseViewModel(ResponseCode.fail);
        }
        /// <summary>
        /// 組織選取器 取得部門清單
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrgPickerDeptInfoModel>> OrgPicker_GetDeptInfo()
        {
            string sql = "select DeptID, DeptName from TB_Org_Dept_Info where status=1 order by DeptName";
            return await AsyncDB.QueryAsync<OrgPickerDeptInfoModel>(str_conn, sql);
        }
        /// <summary>
        /// 組織選取器 取得人員清單
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrgPickerUserInfoModel>> OrgPicker_GetUserInfo(string manager, OrgPickerAccountInfoGetModel model)
        {
            string sql = "select i.UID, i.Name, i.EmployeeID, s.DeptID, di.DeptName, s.Main from TB_Org_Account_Info i join TB_Org_Account_Struct s on i.UID=s.UID"+
                            " join TB_Org_Dept_Info di on s.DeptID = di.DeptID"+
                            " where i.Status = 1 and s.Status=1";

            DynamicParameters _params = new DynamicParameters();


            // 只顯示部門人員
            List<string> ChildrenDeptList = new List<string>();

            if (model.onlyDeptUser)
            {
                string sql_dept = "select DeptID from TB_Org_Account_Struct where UID=@in_manager and Status=1 ";
                DynamicParameters _params_dept = new DynamicParameters();
                _params_dept.Add("@in_manager", manager, DbType.String, size: 255);
                List<string> DeptList = await AsyncDB.QueryAsync<string>(str_conn, sql_dept, _params_dept);

                foreach (var DeptID in DeptList)
                {
                    DynamicParameters _params_child = new DynamicParameters();
                    _params_child.Add("@in_DeptID", DeptID, DbType.String, size: 255);

                    List<OrgGetChildDeptModel> childList = await AsyncDB.QueryAsync<OrgGetChildDeptModel>(str_conn, "SP_Org_GetChildDept", _params_child, type: CommandType.StoredProcedure);

                    if (childList != null && childList.Count > 0)
                    {
                        ChildrenDeptList.AddRange(childList.Select(x => x.DeptID));
                    }
                }

                sql += " and s.DeptID in @in_DeptList ";
                _params.Add("@in_DeptList", ChildrenDeptList);

            }
            else if (model.whichDeptUser != null && model.whichDeptUser.Any())
            {
                foreach (var DeptID in model.whichDeptUser)
                {
                    DynamicParameters _params_child = new DynamicParameters();
                    _params_child.Add("@in_DeptID", DeptID, DbType.String, size: 255);

                    List<OrgGetChildDeptModel> childList = await AsyncDB.QueryAsync<OrgGetChildDeptModel>(str_conn, "SP_Org_GetChildDept", _params_child, type: CommandType.StoredProcedure);

                    if (childList != null && childList.Count > 0)
                    {
                        ChildrenDeptList.AddRange(childList.Select(x => x.DeptID));
                    }
                }

                sql += " and s.DeptID in @in_DeptList ";
                _params.Add("@in_DeptList", ChildrenDeptList);
            }


            return await AsyncDB.QueryAsync<OrgPickerUserInfoModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得職稱清單
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<List<OrgTitleModel>> GetTitleList(OrgTitleGetModel model)
        {
            DynamicParameters _params = new DynamicParameters();

            string sql = "select * from TB_Org_Title where 1=1";

            if (model.Status.HasValue)
            {
                sql += " and Status=@in_status";
                _params.Add("@in_status", model.Status, DbType.Int16);
            }

            sql += " order by Level desc ";

            return await AsyncDB.QueryAsync<OrgTitleModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得公司別清單
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrgCompanyTypeModel>> GetCompanyTypeList()
        {
            string sql = "select CompanyType, TypeName from TB_Org_CompanyType order by CompanyType";
            return await AsyncDB.QueryAsync<OrgCompanyTypeModel>(str_conn, sql);
        }
        /// <summary>
        /// 新增人員
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> AddAccountInfoStruct(string editor, OrgAccountInfoStructAddModel model)
        {
            string uid = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 判斷email重覆
                        string sql_email = "select exists(select 1 from TB_Org_Account_Info where UPPER(Email) = UPPER(@in_Email))";
                        DynamicParameters _params_email = new DynamicParameters();
                        _params_email.Add("@in_Email", model.Email.Trim(), DbType.String, size: 255);

                        bool email_isExists = await conn.QueryFirstOrDefaultAsync<bool>(sql_email, _params_email, transaction);

                        if (email_isExists)
                        {
                            return new ResponseViewModel(ResponseCode.fail, "Email重覆");
                        }


                        // 判斷員工編號 重覆
                        string sql_EmployeeID = "select exists(select 1 from TB_Org_Account_Info where UPPER(EmployeeID) = UPPER(@in_EmployeeID) )";
                        DynamicParameters _params_EmployeeID = new DynamicParameters();
                        _params_EmployeeID.Add("@in_EmployeeID", model.EmployeeID.Trim(), DbType.String, size: 255);

                        bool EmployeeID_isExists = await conn.QueryFirstOrDefaultAsync<bool>(sql_EmployeeID, _params_EmployeeID, transaction);

                        if (EmployeeID_isExists)
                        {
                            return new ResponseViewModel(ResponseCode.fail, "員工編號重覆");
                        }


                        // insert
                        string sql = "insert into TB_Org_Account_Info (UID, Name, Email, EmployeeID, Status, CreateDate, Editor)" +
                         " values(@in_UID, @in_Name, @in_Email, @in_EmployeeID, @in_Status , now(), @in_Editor); " +

                         "insert into TB_Org_Account_Struct (UID, DeptID, Main, TitleID, SignApprover, CompanyType, Status, CreateDate, Editor)" +
                        " values(@in_UID, @in_DeptID, @in_Main, @in_TitleID, @in_SignApprover, @in_CompanyType, @in_Status, now(), @in_Editor); ";

                        DynamicParameters _params = new DynamicParameters();
                        _params.Add("@in_UID", uid, DbType.String, size: 255);
                        _params.Add("@in_Name", model.Name.Trim(), DbType.String, size: 255);
                        _params.Add("@in_Email", model.Email.Trim(), DbType.String, size: 255);
                        _params.Add("@in_EmployeeID", model.EmployeeID.Trim(), DbType.String, size: 255);
                        _params.Add("@in_Status", model.Status, DbType.Int16);
                        _params.Add("@in_Editor", editor, DbType.String, size: 255);
                        _params.Add("@in_DeptID", model.DeptID, DbType.String, size: 255);
                        _params.Add("@in_Main", model.Main, DbType.Int16);
                        _params.Add("@in_TitleID", model.Main, DbType.Int32);
                        _params.Add("@in_SignApprover", model.SignApprover, DbType.Int16);
                        _params.Add("@in_CompanyType", model.CompanyType, DbType.String, size: 50);


                        await conn.ExecuteAsync(sql, _params, transaction);


                        await transaction.CommitAsync();
                        return new ResponseViewModel(ResponseCode.success);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await transaction.RollbackAsync();
                        //throw ex;
                        return new ResponseViewModel(ResponseCode.fail, ex.Message);
                    }
                    finally
                    {
                        //if (conn.State != ConnectionState.Closed) conn.Close();
                        //conn.Dispose();
                    }
                }
            }

            return new ResponseViewModel(ResponseCode.fail);
        }
        /// <summary>
        /// 取得部門資訊/結構
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrgDeptInfoStructModel>> OrgDeptInfoStruct()
        {
            string sql = "select i.DeptID, i.DeptName, s.ParentDept, s.Expand, s.Sort from "+
                            " TB_Org_Dept_Info i join TB_Org_Dept_Struct s on i.DeptID = s.DeptID"+
                            " where i.Status = 1 and s.Status = 1 ";
            return await AsyncDB.QueryAsync<OrgDeptInfoStructModel>(str_conn, sql);
        }
        /// <summary>
        /// 取得人員資訊/結構
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrgAccountInfoStructModel>> OrgAccountInfoStruct()
        {
            string sql =
                        " select i.UID, i.Name, s.DeptID, s.SignApprover, t.Level" +
                        " from TB_Org_Account_Info i left join TB_Org_Account_Struct s on i.UID = s.UID" +
                        " left join TB_Org_Title t on s.TitleID = t.TitleID" +
                        " where i.Status = 1 and s.Status = 1 and s.DeptID is not null";

            return await AsyncDB.QueryAsync<OrgAccountInfoStructModel>(str_conn, sql);
        }
        /// <summary>
        /// 取得人員清單
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrgAccountListModel>> OrgAccountList(OrgAccountListGetModel model)
        {
            DynamicParameters _params = new DynamicParameters();

            string sql =" select i.UID, i.Name, i.Email, i.EmployeeID, i.Status , s.DeptID, di.DeptName, s.SignApprover, t.Title, t.TitleID, s.CompanyType, c.TypeName as CompanyTypeName" +
                        " from TB_Org_Account_Info i " +
                        " left join TB_Org_Account_Struct s on i.UID = s.UID" +
                        " left join TB_Org_Dept_Info di on s.DeptID=di.DeptiD" +
                        " left join TB_Org_Title t on s.TitleID = t.TitleID" +
                        " left join TB_Org_CompanyType c on s.CompanyType=c.companyType" +
                        " where 1=1 and (s.Main is null or s.Main=1)";


            if (!string.IsNullOrWhiteSpace(model.EmployeeID))
            {
                sql += " and UPPER(i.EmployeeID) like @in_EmployeeID ";
                _params.Add("@in_EmployeeID", $"%{model.EmployeeID.Trim().ToUpper()}%", DbType.String, size: 255);
            }


            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                sql += " and UPPER(i.Name) like @in_Name ";
                _params.Add("@in_Name", $"%{model.Name.Trim().ToUpper()}%", DbType.String, size: 255);
            }

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                sql += " and UPPER(i.Email) like @in_Email ";
                _params.Add("@in_Email", $"%{model.Email.Trim().ToUpper()}%", DbType.String, size: 255);
            }

            
            if (model.Status.HasValue)
            {
                sql += " and i.Status= @in_Status ";
                _params.Add("@in_Status", model.Status, DbType.Int16);
            }


            return await AsyncDB.QueryAsync<OrgAccountListModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得部門清單
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrgDeptListModel>> GetDeptList()
        {
            string sql =" select i.DeptID, i.DeptName, s.DeptLevelID, l.LevelName, s.Sort, s.Status, s.Expand, s.ParentDept," +
                        " (select DeptName from TB_Org_Dept_Info where DeptID = s.ParentDept) as ParentDeptName" +
                        " from TB_Org_Dept_Info i join TB_Org_Dept_Struct s on i.DeptID = s.DeptID" +
                        " join TB_Org_Dept_Level l on s.DeptLevelID = l.DeptLevelID";

            return await AsyncDB.QueryAsync<OrgDeptListModel>(str_conn, sql);
        }
        public async Task<DeptInfoStructEditModel> GetDeptInfoStruct(string deptID)
        {
            string sql =
                        " select i.DeptID, i.DeptName, s.DeptLevelID, s.Status, s.ParentDept," +
                        " (select DeptName from TB_Org_Dept_Info where DeptID = s.ParentDept) as ParentDeptName" +
                        "  from TB_Org_Dept_Info i join TB_Org_Dept_Struct s on i.DeptID = s.DeptID" +
                        " where i.DeptID = @in_DeptID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_DeptID", deptID, DbType.String, size: 255);

            return await AsyncDB.QueryFirstOrDefaultAsync<DeptInfoStructEditModel>(str_conn, sql, _params);
        }
        public async Task<OrgAccountInfoModel> GetAccountInfo(string uid)
        {
            string sql = " select * from TB_Org_Account_Info where UID = @in_UID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_UID", uid, DbType.String, size: 255);

            return await AsyncDB.QueryFirstOrDefaultAsync<OrgAccountInfoModel>(str_conn, sql, _params);
        }
        public async Task<List<OrgAccountStructModel>> GetAccountStructList(string uid)
        {
            string sql =
                        " select s.UID, s.DeptID, di.DeptName, s.Main, t.TitleID, t.Title, s.SignApprover, ct.CompanyType, ct.TypeName as CompanyTypeName, s.Status, s.Agent," +
                        " (select Name from TB_Org_Account_Info where UID = s.Agent limit 1) as AgentName" +
                        " from TB_Org_Account_Struct s join TB_Org_Dept_Info di on s.DeptID = di.DeptID" +
                        " join TB_Org_Title t on s.TitleID = t.TitleID" +
                        " join TB_Org_CompanyType ct on s.CompanyType = ct.CompanyType" +
                        " where UID = @in_UID order by s.CreateDate";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_UID", uid, DbType.String, size: 255);

            return await AsyncDB.QueryAsync<OrgAccountStructModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 修改人員資料
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> EditUserInfo(string editor, OrgAccountInfoModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 判斷email重覆
                        string sql_email = "select exists(select 1 from TB_Org_Account_Info where UPPER(Email) = UPPER(@in_Email) and UID!=@in_UID )";
                        DynamicParameters _params_email = new DynamicParameters();
                        _params_email.Add("@in_UID", model.UID, DbType.String, size: 255);
                        _params_email.Add("@in_Email", model.Email.Trim(), DbType.String, size: 255);

                        bool email_isExists = await conn.QueryFirstOrDefaultAsync<bool>(sql_email, _params_email, transaction);

                        if (email_isExists)
                        {
                            return new ResponseViewModel(ResponseCode.fail, "Email重覆");
                        }


                        // 判斷員工編號 重覆
                        string sql_EmployeeID = "select exists(select 1 from TB_Org_Account_Info where UPPER(EmployeeID) = UPPER(@in_EmployeeID) and UID!=@in_UID )";
                        DynamicParameters _params_EmployeeID = new DynamicParameters();
                        _params_EmployeeID.Add("@in_UID", model.UID, DbType.String, size: 255);
                        _params_EmployeeID.Add("@in_EmployeeID", model.EmployeeID.Trim(), DbType.String, size: 255);

                        bool EmployeeID_isExists = await conn.QueryFirstOrDefaultAsync<bool>(sql_EmployeeID, _params_EmployeeID, transaction);

                        if (EmployeeID_isExists)
                        {
                            return new ResponseViewModel(ResponseCode.fail, "員工編號重覆");
                        }


                        // update
                        string sql = " UPDATE TB_Org_Account_Info" +
                                    " SET" +
                                    " Name = @in_Name," +
                                    " Email = @in_Email," +
                                    " EmployeeID = @in_EmployeeID," +
                                    " Status = @in_Status," +
                                    " UpdateDate = now()," +
                                    " Editor = @in_Editor" +
                                    " WHERE UID = @in_UID ; ";


                        DynamicParameters _params = new DynamicParameters();
                        _params.Add("@in_UID", model.UID, DbType.String, size: 255);
                        _params.Add("@in_Name", model.Name.Trim(), DbType.String, size: 255);
                        _params.Add("@in_Email", model.Email.Trim(), DbType.String, size: 255);
                        _params.Add("@in_EmployeeID", model.EmployeeID.Trim(), DbType.String, size: 255);
                        _params.Add("@in_Status", model.Status, DbType.Int16);
                        _params.Add("@in_Editor", editor, DbType.String, size: 255);



                        await conn.ExecuteAsync(sql, _params, transaction);


                        await transaction.CommitAsync();
                        return new ResponseViewModel(ResponseCode.success);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await transaction.RollbackAsync();
                        //throw ex;
                        return new ResponseViewModel(ResponseCode.fail, ex.Message);
                    }
                    finally
                    {
                        //if (conn.State != ConnectionState.Closed) conn.Close();
                        //conn.Dispose();
                    }
                }
            }

            return new ResponseViewModel(ResponseCode.fail);
        }
        /// <summary>
        /// 新增/修改 人員所屬部門
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> AddAccountStruct(string editor, OrgAccountStructAddModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 判斷部門重覆
                        string sql_dept = "select exists(select 1 from TB_Org_Account_Struct where UID=@in_UID and DeptID = @in_DeptID and DeptID!=@in_oldDeptID )";

                        if (model.isAdd)
                        {
                            sql_dept = "select exists(select 1 from TB_Org_Account_Struct where UID=@in_UID and DeptID = @in_DeptID )";
                        }

                        DynamicParameters _params_dept = new DynamicParameters();
                        _params_dept.Add("@in_UID", model.UID, DbType.String, size: 255);
                        _params_dept.Add("@in_DeptID", model.DeptID, DbType.String, size: 255);
                        _params_dept.Add("@in_oldDeptID", model.oldDeptID, DbType.String, size: 255);


                        bool dept_isExists = await conn.QueryFirstOrDefaultAsync<bool>(sql_dept, _params_dept, transaction);

                        if (dept_isExists)
                        {
                            return new ResponseViewModel(ResponseCode.fail, "部門重覆");
                        }


                        // insert
                        if (model.isAdd)
                        {
                            string sql = " insert into TB_Org_Account_Struct (UID, DeptID, Main, TitleID, SignApprover, CompanyType, Status, CreateDate, Editor)" +
                                            " values(@in_UID, @in_DeptID, @in_Main, @in_TitleID, @in_SignApprover, @in_CompanyType, @in_Status, now(), @in_Editor); ";

                            DynamicParameters _params = new DynamicParameters();
                            _params.Add("@in_UID", model.UID, DbType.String, size: 255);
                            _params.Add("@in_DeptID", model.DeptID, DbType.String, size: 255);
                            _params.Add("@in_Main", model.Main, DbType.Int16);
                            _params.Add("@in_TitleID", model.TitleID, DbType.Int32);
                            _params.Add("@in_SignApprover", model.SignApprover, DbType.Int16);
                            _params.Add("@in_CompanyType", model.CompanyType, DbType.String, size: 50);
                            _params.Add("@in_Status", model.Status, DbType.Int16);
                            _params.Add("@in_Editor", editor, DbType.String, size: 255);

                            await conn.ExecuteAsync(sql, _params, transaction);
                        }
                        else
                        {
                            string sql = "UPDATE TB_Org_Account_Struct" +
                                         " SET" +
                                         " DeptID = @in_DeptID," +
                                         " Main = @in_Main," +
                                         " TitleID = @in_TitleID," +
                                         " SignApprover = @in_SignApprover," +
                                         " Agent = @in_Agent," +
                                         " CompanyType = @in_CompanyType," +
                                         " Status = @in_Status," +
                                         " UpdateDate = now()," +
                                         " Editor = @in_Editor" +
                                         " WHERE UID = @in_UID AND DeptID = @in_oldDeptID";


                            DynamicParameters _params = new DynamicParameters();
                            _params.Add("@in_UID", model.UID, DbType.String, size: 255);
                            _params.Add("@in_oldDeptID", model.oldDeptID, DbType.String, size: 255);
                            _params.Add("@in_DeptID", model.DeptID, DbType.String, size: 255);
                            _params.Add("@in_Main", model.Main, DbType.Int16);
                            _params.Add("@in_TitleID", model.TitleID, DbType.Int32);
                            _params.Add("@in_SignApprover", model.SignApprover, DbType.Int16);
                            _params.Add("@in_Agent", model.Agent, DbType.String, size: 255);
                            _params.Add("@in_CompanyType", model.CompanyType, DbType.String, size: 50);
                            _params.Add("@in_Status", model.Status, DbType.Int16);
                            _params.Add("@in_Editor", editor, DbType.String, size: 255);

                            await conn.ExecuteAsync(sql, _params, transaction);
                        }

                        await transaction.CommitAsync();
                        return new ResponseViewModel(ResponseCode.success);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await transaction.RollbackAsync();
                        //throw ex;
                        return new ResponseViewModel(ResponseCode.fail, ex.Message);
                    }
                    finally
                    {
                        //if (conn.State != ConnectionState.Closed) conn.Close();
                        //conn.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// 新增/修改部門層級
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> AddDeptLevel(string editor, OrgDeptLevelAddModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 判斷部門層級重覆
                        string sql_level = "select exists(select 1 from TB_Org_Dept_Level where UPPER(LevelName) = UPPER(@in_LevelName) and DeptLevelID!=@in_DeptLevelID )";

                        if (model.isAdd)
                        {
                            sql_level = "select exists(select 1 from TB_Org_Dept_Level where UPPER(LevelName) = UPPER(@in_LevelName) )";
                        }

                        DynamicParameters _params_level = new DynamicParameters();
                        _params_level.Add("@in_DeptLevelID", model.DeptLevelID, DbType.String, size: 50);
                        _params_level.Add("@in_LevelName", model.LevelName.Trim(), DbType.String, size: 50);


                        bool title_isExists = await conn.QueryFirstOrDefaultAsync<bool>(sql_level, _params_level, transaction);

                        if (title_isExists)
                        {
                            return new ResponseViewModel(ResponseCode.fail, "部門層級重覆");
                        }


                        // insert
                        if (model.isAdd)
                        {
                            string sql = "INSERT INTO TB_Org_Dept_Level (LevelName, Level, Status, CreateDate, Editor)" +
                                        " VALUES(@in_LevelName, @in_Level, @in_Status, now(), @in_Editor); ";

                            DynamicParameters _params = new DynamicParameters();
                            _params.Add("@in_LevelName", model.LevelName.Trim(), DbType.String, size: 50);
                            _params.Add("@in_Level", model.Level, DbType.Int32);
                            _params.Add("@in_Status", model.Status, DbType.Int16);
                            _params.Add("@in_Editor", editor, DbType.String, size: 255);

                            await conn.ExecuteAsync(sql, _params, transaction);
                        }
                        else
                        {
                            string sql = "UPDATE TB_Org_Dept_Level" +
                                        " SET" +
                                        " LevelName = @in_LevelName," +
                                        " Level = @in_Level," +
                                        " Status = @in_Status," +
                                        " UpdateDate = now()," +
                                        " Editor = @in_Editor" +
                                        " WHERE DeptLevelID = @in_DeptLevelID;";

                            DynamicParameters _params = new DynamicParameters();
                            _params.Add("@in_DeptLevelID", model.DeptLevelID, DbType.String, size: 50);
                            _params.Add("@in_LevelName", model.LevelName.Trim(), DbType.String, size: 50);
                            _params.Add("@in_Level", model.Level, DbType.Int32);
                            _params.Add("@in_Status", model.Status, DbType.Int16);
                            _params.Add("@in_Editor", editor, DbType.String, size: 255);

                            await conn.ExecuteAsync(sql, _params, transaction);
                        }


                        await transaction.CommitAsync();
                        return new ResponseViewModel(ResponseCode.success);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await transaction.RollbackAsync();
                        //throw ex;
                        return new ResponseViewModel(ResponseCode.fail, ex.Message);
                    }
                    finally
                    {
                        //if (conn.State != ConnectionState.Closed) conn.Close();
                        //conn.Dispose();
                    }
                }
            }

        }

        /// <summary>
        /// 新增/修改職稱
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseViewModel> AddTitle(string editor, OrgTitleAddModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 判斷職稱重覆
                        string sql_title = "select exists(select 1 from TB_Org_Title where UPPER(Title) = UPPER(@in_Title) and TitleID!=@in_TitleID )";

                        if (model.isAdd)
                        {
                            sql_title = "select exists(select 1 from TB_Org_Title where UPPER(Title) = UPPER(@in_Title) )";
                        }

                        DynamicParameters _params_title = new DynamicParameters();
                        _params_title.Add("@in_TitleID", model.TitleID, DbType.String, size: 255);
                        _params_title.Add("@in_Title", model.Title.Trim(), DbType.String, size: 50);


                        bool title_isExists = await conn.QueryFirstOrDefaultAsync<bool>(sql_title, _params_title, transaction);

                        if (title_isExists)
                        {
                            return new ResponseViewModel(ResponseCode.fail, "職稱重覆");
                        }


                        if (model.isAdd)
                        {

                            string sql = "INSERT INTO TB_Org_Title ( Title , Level , Status , CreateDate , Editor )" +
                                        " VALUES(@in_Title, @in_Level, @in_Status, now(), @in_Editor); ";

                            DynamicParameters _params = new DynamicParameters();
                            _params.Add("@in_Title", model.Title.Trim(), DbType.String, size: 50);
                            _params.Add("@in_Level", model.Level, DbType.Int32);
                            _params.Add("@in_Status", model.Status, DbType.Int16);
                            _params.Add("@in_Editor", editor, DbType.String, size: 255);

                            await conn.ExecuteAsync(sql, _params, transaction);

                        }
                        else
                        {
                            string sql = "UPDATE TB_Org_Title" +
                                        " SET" +
                                        " Title = @in_Title," +
                                        " Level = @in_Level," +
                                        " Status = @in_Status," +
                                        " UpdateDate = now()," +
                                        " Editor = @in_Editor" +
                                        " WHERE TitleID = @in_TitleID;";

                            DynamicParameters _params = new DynamicParameters();
                            _params.Add("@in_TitleID", model.TitleID, DbType.String, size: 50);
                            _params.Add("@in_Title", model.Title.Trim(), DbType.String, size: 50);
                            _params.Add("@in_Level", model.Level, DbType.Int32);
                            _params.Add("@in_Status", model.Status, DbType.Int16);
                            _params.Add("@in_Editor", editor, DbType.String, size: 255);

                            await conn.ExecuteAsync(sql, _params, transaction);

                        }

                        await transaction.CommitAsync();
                        return new ResponseViewModel(ResponseCode.success);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await transaction.RollbackAsync();
                        //throw ex;
                        return new ResponseViewModel(ResponseCode.fail, ex.Message);
                    }
                    finally
                    {
                        //if (conn.State != ConnectionState.Closed) conn.Close();
                        //conn.Dispose();
                    }
                }
            }

        }
        #endregion

        #region 權限
        /// <summary>
        /// 權限管理 取得使用者權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public async Task<List<MenuAuthListModel>> GetUserMenuAuthList(string manager, MenuAuthListGetModel model, bool isAdmin)
        {
            string sql = string.Empty;

            // admin
            if (isAdmin)
            {
                // 通用功能
                if (model.AppID == ((int)AuthTypeEnum.auth_private).ToString())
                {
                    sql = " select m.MenuID, m.MenuName, IFNULL(p.Status, 0) as Status , m.MainMenu, m.Sort " +
                            " from TB_Menu m " +
                            " left join TB_App_Permission p on m.MenuID=p.MenuID and p.UID=@in_UID " +
                            " where m.AuthType=2 ";
                }
                else
                {
                    // 分遊戲權限
                    sql = " select m.MenuID, m.MenuName, IFNULL(p.Status, 0) as Status , m.MainMenu, m.Sort " +
                            " from TB_Menu m " +
                            " left join TB_App_Permission p on m.MenuID=p.MenuID and p.UID=@in_UID  and p.AppID=@in_AppID " +
                            " where m.AuthType=3 ";
                }
            }
            else
            {
                // 通用功能
                if (model.AppID == ((int)AuthTypeEnum.auth_private).ToString())
                {
                    sql = " select m.MenuID, m.MenuName, IFNULL(p.Status, 0) as Status, m.MainMenu, m.Sort" +
                        " from TB_Menu m" +
                        " left join TB_App_Permission p on m.MenuID = p.MenuID and p.UID = @in_UID and p.AppID = @in_AppID" +
                        " where m.AuthType = 2" +
                        " and m.MenuID in (select MenuID from TB_App_Permission where UID = @in_manager and AppID = @in_AppID and Status = 1)";
                }
                else
                {
                    // 分遊戲權限
                    sql = " select m.MenuID, m.MenuName, IFNULL(p.Status, 0) as Status, m.MainMenu, m.Sort" +
                        " from TB_Menu m" +
                        " left join TB_App_Permission p on m.MenuID = p.MenuID and p.UID = @in_UID and p.AppID = @in_AppID" +
                        " where m.AuthType = 3" +
                        " and m.MenuID in (select MenuID from TB_App_Permission where UID = @in_manager and AppID = @in_AppID and Status = 1)";
                }
            }


            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_UID", model.UID, DbType.String, size: 255);
            _params.Add("@in_AppID", model.AppID, DbType.String, size: 255);
            _params.Add("@in_manager", manager, DbType.String, size: 255);

            return await AsyncDB.QueryAsync<MenuAuthListModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 權限管理 修改使用者權限
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseCode> SetUserMenuAuth(string editor, MenuAuthSetModel model)
        {
            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string menuList_All = string.Join(",", model.MenuListAll);
                        string newAuth = string.Join(",", model.MenuList);
                        

                        // Log
                        string logsql = "INSERT INTO `BackEndV2DB`.`TB_App_Permission_Log`" +
                                        " (`MenuList`, `UID`,`AppID`,`OldAuth`,`NewAuth`,`UpdateDate`,`Editor`) " +
                                        " VALUES (@in_MenuList, @in_UID, @in_AppID, ( SELECT GROUP_CONCAT(MenuID SEPARATOR ',') FROM TB_App_Permission where UID=@in_UID and AppID=@in_AppID ) , @in_newAuth, now(), @in_editor);";

                        DynamicParameters _params_log = new DynamicParameters();
                        _params_log.Add("@in_MenuList", menuList_All, DbType.String, size: 1000);
                        _params_log.Add("@in_AppID", model.AppID, DbType.String, size: 50);
                        _params_log.Add("@in_UID", model.UID, DbType.String, size: 255);
                        _params_log.Add("@in_newAuth", newAuth, DbType.String, size: 1000);
                        _params_log.Add("@in_editor", editor, DbType.String, size: 255);

                        await conn.ExecuteAsync(logsql, _params_log, transaction);




                        // 先清除權限
                        string sql = $"DELETE FROM TB_App_Permission where UID=@in_UID and AppID=@in_AppID and MenuID in ({menuList_All}) ";

                        DynamicParameters _params = new DynamicParameters();
                        _params.Add("@in_UID", model.UID, DbType.String, size: 255);
                        _params.Add("@in_AppID", model.AppID, DbType.String, size: 255);

                        await conn.ExecuteAsync(sql, _params, transaction);


                        // 新增權限
                        if (model.MenuList != null && model.MenuList.Count > 0)
                        {
                            foreach (var menuID in model.MenuList)
                            {

                                string sql_insert = " INSERT INTO TB_App_Permission (`UID`,`MenuID`,`AppID`,`Status`)" +
                                                    " VALUES(@in_UID, @in_MenuID, @in_AppID, 1 ) ; ";

                                DynamicParameters _params_insert = new DynamicParameters();
                                _params_insert.Add("@in_UID", model.UID, DbType.String, size: 255);
                                _params_insert.Add("@in_AppID", model.AppID, DbType.String, size: 255);
                                _params_insert.Add("@in_MenuID", menuID, DbType.Int32);

                                await conn.ExecuteAsync(sql_insert, _params_insert, transaction);
                            }

                        }

                        await transaction.CommitAsync();
                        return ResponseCode.success;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await transaction.RollbackAsync();
                        //throw ex;
                        return ResponseCode.fail;
                    }
                    finally
                    {
                        //if (conn.State != ConnectionState.Closed) conn.Close();
                        //conn.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// 依url 取得menuid
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<int> GetMenuIDByURL(string url)
        {
            string sql = "select MenuID from TB_Menu where Url=@in_url";
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_url", url, DbType.String, size: 255);

            return await AsyncDB.QueryFirstOrDefaultAsync<int>(str_conn, sql, _params);
        }
        /// <summary>
        /// 檢查權限
        /// </summary>
        /// <param name="menuID"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<bool> AuthCheck(int menuID, string uid)
        {
            string sql = "select exists( "+
                            " select 1 from TB_Menu m left" +
                            " join TB_App_Permission p on p.MenuID = m.MenuID" +
                            " where (m.MenuID = @in_MenuID and m.AuthType = 1 ) or (p.UID = @in_UID and m.MenuID = @in_MenuID and p.Status = 1)" +
                        " ) ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_UID", uid, DbType.String, size: 255);
            _params.Add("@in_MenuID", menuID, DbType.Int32);

            return await AsyncDB.QueryFirstOrDefaultAsync<bool>(str_conn, sql, _params);
        }
        

        #endregion
    }
}
