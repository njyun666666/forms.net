using FormsNet.DB.DBClass;
using FormsNet.DB.IDB;
using FormsNet.Enums;
using FormsNet.Models.Form;
using FormsNet.Models.Search;
using FormsNet.Models.Sign;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB
{
    public class BackEndV2DB_Form : IBackEndV2DB_Form
    {
        public readonly string DbName = "BackEndV2DB";
        public string str_conn;

        public IDBConnection _dBConnection;

        IBackEndV2DB_File db_File;

        public BackEndV2DB_Form(IDBConnection dBConnection, IBackEndV2DB_File backEndV2DB_File)
        {
            _dBConnection = dBConnection;
            str_conn = _dBConnection.Connection(DbName);
            db_File = backEndV2DB_File;
        }
        /// <summary>
        /// 取得表單類別清單
        /// </summary>
        /// <returns></returns>
        public async Task<List<FormTypeModel>> GetFormTypeList(Int16 status = 1)
        {
            string statusStr = "1";

            if (status == 0)
            {
                statusStr = "1,0";
            }

            string sql = $"select * from TB_Form_Type where Status in ({statusStr}) order by Sort";
            return await AsyncDB.QueryAsync<FormTypeModel>(str_conn, sql);
        }
        /// <summary>
        /// 取得表單清單
        /// </summary>
        /// <returns></returns>
        public async Task<List<FormSettingModel>> GetFormSettingList(Int16 status = 1)
        {
            string statusStr = "1";

            if (status == 0)
            {
                statusStr = "1,0";
            }

            string sql = $"select * from TB_Form_Setting where Status in ({statusStr}) order by Sort";
            return await AsyncDB.QueryAsync<FormSettingModel>(str_conn, sql);
        }
        public async Task<string> GetFormClass(string formID)
        {
            string sql = "select FormClass from TB_Form_List where FormID=@in_formID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_formID", formID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<string>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得表單設定
        /// </summary>
        /// <param name="FormClass"></param>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<FormSettingModel> GetFormSetting(string FormClass, string FormID)
        {
            string sql = "select * from TB_Form_Setting where FormClass=@in_FormClass ";

            if (string.IsNullOrWhiteSpace(FormClass))
            {
                sql = "select * from TB_Form_Setting where FormClass=(select FormClass from TB_Form_List  where FormID=@in_FormID limit 1)";
            }

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormClass", FormClass, DbType.String);
            _params.Add("@in_FormID", FormID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<FormSettingModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得申請人清單
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<ApplicantListModel>> GetApplicantList(string uid)
        {
            string sql = " select i.UID, i.Name, di.DeptID, di.DeptName from TB_Org_Account_Info i join TB_Org_Account_Struct s on i.UID = s.UID" +
                        " join TB_Org_Dept_Info di on s.DeptID = di.DeptID" +
                        " where  i.UID = @in_uid  and i.Status = 1 and s.Status = 1 and di.Status = 1" +
                        " order by i.Name, s.Main desc, di.DeptName";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String, size: 255);

            return await AsyncDB.QueryAsync<ApplicantListModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得表單級別
        /// </summary>
        /// <returns></returns>
        public async Task<List<FormLevelModel>> GetFormLevel()
        {
            string sql = "select * from TB_Form_Level";
            return await AsyncDB.QueryAsync<FormLevelModel>(str_conn, sql);
        }
        /// <summary>
        /// 取得填單人資料
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<FormListModel> GetFillerInfo(string uid)
        {
            string sql = "select i.UID as FillerID, i.Name as FillerName, di.DeptID as FillerDeptID, di.DeptName as FillerDept from TB_Org_Account_Info i join TB_Org_Account_Struct s on i.UID=s.UID join TB_Org_Dept_Info di on s.DeptID=di.DeptID" +
                        " where i.UID = @in_uid and i.Status = 1 and s.Status = 1 and di.Status = 1" +
                        " order by s.Main desc limit 1";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String, size: 255);

            return await AsyncDB.QueryFirstOrDefaultAsync<FormListModel>(str_conn, sql, _params);
        }
        public async Task<int> DeleteDraft(string uid, string formID)
        {
            string sql = "UPDATE `TB_Form_List` SET `Status` = -1 WHERE `FormID` = @in_formID and `FillerID`=@in_uid ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String);
            _params.Add("@in_formID", formID, DbType.String);

            return await AsyncDB.ExecuteAsync(str_conn, sql, _params);
        }
        /// <summary>
        /// 草稿 寫入主表, 附件轉正式
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="fillerID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Applicant(MySqlConnection conn, MySqlTransaction transaction, FormListModel model)
        {
            // TB_Fom_List
            // 先查有沒有 FormID 存在
            string sql_exists = "select exists(select 1 from TB_Form_List where FormID=@in_FormID);";
            DynamicParameters params_exists = new DynamicParameters();
            params_exists.Add("@in_FormID", model.FormID, DbType.String);
            bool exists = await conn.QueryFirstOrDefaultAsync<bool>(sql_exists, params_exists, transaction);


            // 寫入主表
            DynamicParameters params_data = new DynamicParameters();
            params_data.Add("@in_FormID", model.FormID, DbType.String);
            params_data.Add("@in_FormClass", model.FormClass, DbType.String);
            params_data.Add("@in_FillerID", model.FillerID, DbType.String);
            params_data.Add("@in_FillerName", model.FillerName, DbType.String);
            params_data.Add("@in_FillerDeptID", model.FillerDeptID, DbType.String);
            params_data.Add("@in_FillerDept", model.FillerDept, DbType.String);
            params_data.Add("@in_ApplicantID", model.ApplicantID, DbType.String);
            params_data.Add("@in_ApplicantName", model.ApplicantName, DbType.String);
            params_data.Add("@in_ApplicantDeptID", model.ApplicantDeptID, DbType.String);
            params_data.Add("@in_ApplicantDept", model.ApplicantDept, DbType.String);
            params_data.Add("@in_ApplicantDate", model.ApplicantDate, DbType.DateTime);
            params_data.Add("@in_LevelID", model.LevelID, DbType.Int32);
            params_data.Add("@in_FileGroupID", model.FileGroupID, DbType.String);
            params_data.Add("@in_Status", model.Status, DbType.Int16);

            string sql_data = "";

            if (exists)
            {
                // update
                sql_data = " UPDATE `TB_Form_List`" +
                            " SET" +
                            " `FillerID` =  @in_FillerID," +
                            " `FillerName` =  @in_FillerName," +
                            " `FillerDeptID` =  @in_FillerDeptID," +
                            " `FillerDept` =  @in_FillerDept," +
                            " `ApplicantID` =  @in_ApplicantID," +
                            " `ApplicantName` = @in_ApplicantName," +
                            " `ApplicantDeptID` = @in_ApplicantDeptID," +
                            " `ApplicantDept` = @in_ApplicantDept," +
                            " `ApplicantDate` = @in_ApplicantDate," +
                            " `LevelID` = @in_LevelID," +
                            " `Status` = @in_Status," +
                            " `UpdateDate` = now()" +
                            "  WHERE `FormID` =  @in_FormID and `FillerID`= @in_FillerID ;";

            }
            else
            {
                // insert
                sql_data = " INSERT INTO `TB_Form_List`" +
                            " (`FormID`,`FormClass`,`FillerID`,`FillerName`,`FillerDeptID`, `FillerDept`,`ApplicantID`,`ApplicantName`,`ApplicantDeptID`,`ApplicantDept`,`ApplicantDate`,`LevelID`,`Status`, `UpdateDate` ,`FileGroupID`)" +
                            " VALUES" +
                            " ( @in_FormID, @in_FormClass, @in_FillerID, @in_FillerName, @in_FillerDeptID, @in_FillerDept, @in_ApplicantID, @in_ApplicantName, @in_ApplicantDeptID, @in_ApplicantDept, @in_ApplicantDate, @in_LevelID, @in_Status, now(), @in_FileGroupID);";
            }

            int main = await conn.ExecuteAsync(sql_data, params_data, transaction);

            if (main == 0)
            {
                return main;
            }

            // 附件轉正式
            await db_File.FileToRelease(conn, transaction, model.FillerID, model.FileGroupID, model.ApplicantDate);

            return 1;
        }
        /// <summary>
        /// 更新表單簽核結果
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="FormID"></param>
        /// <param name="signResult"></param>
        /// <returns></returns>
        public async Task<int> SetResuult(MySqlConnection conn, MySqlTransaction transaction, string FormID, SignResultTypeEnum signResult)
        {
            string sql = " UPDATE `TB_Form_List`" +
                            " SET" +
                            " `ResultID` = @in_ResultID " +
                            "  WHERE `FormID` =  @in_FormID  ;";

            // 所有步驟結束
            sql += " UPDATE `TB_Sign_Log` SET `SSIDFinish` = 1 WHERE `FormID` = @in_FormID ;";


            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", FormID, DbType.String);
            _params.Add("@in_ResultID", (int)signResult, DbType.Int32);

            return await conn.ExecuteAsync(sql, _params, transaction);
        }
        /// <summary>
        /// 設定表單狀態
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public async Task<int> SetStatus(string FormID, int Status)
        {
            string sql = "update TB_Form_List set Status=@in_Status where FormID=@in_FormID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", FormID, DbType.String);
            _params.Add("@in_Status", Status, DbType.Int32);

            return await AsyncDB.ExecuteAsync(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得草稿清單
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<DraftListModel>> GetDraftList(string uid)
        {
            string sql = " select l.FormID, l.UpdateDate, l.FormClass, s.FormName from TB_Form_List l join TB_Form_Setting s on l.FormClass = s.FormClass" +
                        " where l.FillerID = @in_uid and l.Status = 0 and s.Status = 1" +
                        " order by l.UpdateDate";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String);

            return await AsyncDB.QueryAsync<DraftListModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 表單是否啟用
        /// </summary>
        /// <param name="formClass"></param>
        /// <returns></returns>
        public async Task<bool> FormClassEnabled(string formClass)
        {
            string sql = "select exists (" +
                         " select 1 from TB_Form_Setting where FormClass=@in_formClass and Status=1" +
                         " )";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_formClass", formClass, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<bool>(str_conn, sql, _params);
        }
        /// <summary>
        /// 表單查看權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<bool> FormAuth(string uid, string formID)
        {
            string sql = "select FN_FormAuth(@in_uid, @in_formID)";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String);
            _params.Add("@in_formID", formID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<bool>(str_conn, sql, _params);
        }
        /// <summary>
        /// 草稿權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<bool> DraftAuth(string uid, string formID)
        {
            string sql = "select exists (" +
                         " select 1 from TB_Form_List l join TB_Form_Setting s on l.FormClass = s.FormClass" +
                         " where l.FormID = @in_formID and l.FillerID = @in_uid and l.Status = 0 and s.Status = 1" +
                         " )";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String);
            _params.Add("@in_formID", formID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<bool>(str_conn, sql, _params);
        }
        /// <summary>
        /// 表單權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<bool> ApplicantAuth(string uid, string formID)
        {
            string sql = "select auth from (" +
                            " select count(1)," +
                            " 	if(FormID is null, 1," +
                            " 		if(FillerID=@in_uid and Status=0, 1, 0)" +
                            " 	) as auth" +
                            " 	from TB_Form_List where FormID = @in_formID " +
                        " ) as a";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String);
            _params.Add("@in_formID", formID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<bool>(str_conn, sql, _params);
        }
        /// <summary>
        /// 簽核權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="signID"></param>
        /// <returns></returns>
        public async Task<bool> SignAuth(string uid, SignAuthModel model)
        {
            string sql = "select exists (" +
                            " select 1 from TB_Sign_Log where ID=@in_SignID and FormID=@in_FormID and ApproverID=@in_uid and Status=0 and SSIDFinish=0 " +
                        " ) ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String);
            _params.Add("@in_SignID", model.ID, DbType.Int64);
            _params.Add("@in_FormID", model.FormID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<bool>(str_conn, sql, _params);
        }
        /// <summary>
        /// 撤回權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> SignCancelAuth(string uid, SignAuthModel model)
        {
            string sql = "select exists (" +
                            " select 1 from TB_Form_List where FormID=@in_FormID and ApplicantID=@in_uid and ResultID is null " +
                        " ) ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String);
            _params.Add("@in_FormID", model.FormID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<bool>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得待簽核清單
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<ApproverInfoModel>> GetApproverList(string uid)
        {
            string sql = "select s.ID, l.LevelID, l.LevelNumber, l.Level, s.ArrivalDate, f.Serial, f.ApplicantName, f.ApplicantDept, f.FormID, fs.FormName" +
                         " from TB_Sign_Log s join TB_Form_List f on s.FormID = f.FormID join TB_Form_Level l on f.LevelID = l.LevelID join TB_Form_Setting fs on f.FormClass=fs.FormClass" +
                         " where s.ApproverID = @in_uid and s.Status = 0 and s.SSIDFinish = 0" +
                         "  order by l.LevelNumber desc, s.ArrivalDate";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String);

            return (await AsyncDB.QueryAsync<ApproverInfoModel>(str_conn, sql, _params)).ToList();
        }
        /// <summary>
        /// 取得進行中表單
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<ApplicantInfoModel>> GetApplicantFormList(string uid)
        {
            string sql = "select  l.LevelID, l.LevelNumber, l.Level, f.FormID, fs.FormName, f.Serial, f.ApplicantDate" +
                        "  from TB_Form_List f join TB_Form_Level l on f.LevelID = l.LevelID" +
                        "  join TB_Form_Setting fs on f.FormClass = fs.FormClass" +
                        " where f.ApplicantID = @in_uid and f.Status = 1 and f.ResultID is null";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String);

            return (await AsyncDB.QueryAsync<ApplicantInfoModel>(str_conn, sql, _params)).ToList();
        }
        /// <summary>
        /// 查詢表單
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<List<FormInfoListModel>> GetFormInfoList(string uid, SearchModel model)
        {
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_UID", uid, DbType.String);

            string sql = "select  l.LevelID, l.LevelNumber, l.Level, f.FormID, fs.FormName, f.Serial, f.ApplicantName, f.ApplicantDept, f.ApplicantDate, r.ResultID, r.Result , " +
                        "  FN_GetNowStepApproverJSON(f.FormID) as NowStepApproverJSON" +
                        "  from TB_Form_List f" +
                        "  join TB_Form_Level l on f.LevelID = l.LevelID" +
                        "  join TB_Form_Setting fs on f.FormClass = fs.FormClass" +
                        "  left join TB_Sign_ResultType r on f.ResultID = r.ResultID" +
                        " where f.Status = 1";


            if (model.formClass != null && model.formClass.Any())
            {
                sql += "  and f.FormClass in @in_FormClass ";
                _params.Add("@in_FormClass", model.formClass);
            }

            if (model.startDate.HasValue)
            {
                sql += "  and f.ApplicantDate >= @in_StartDate " ;
                _params.Add("@in_StartDate", model.startDate, DbType.DateTime);
            }

            if (model.endDate.HasValue)
            {
                sql += "  and f.ApplicantDate < @in_EndDate ";
                _params.Add("@in_EndDate", model.endDate.Value.AddDays(1), DbType.DateTime);
            }

            if (!string.IsNullOrWhiteSpace(model.serial))
            {
                model.serial = model.serial.Trim();
                sql += "  and f.Serial like CONCAT('%', @in_Serial ,'%')  ";
                _params.Add("@in_Serial", model.serial, DbType.String);
            }


            List<string> applicants = null;
            if (model.applicants != null && model.applicants.Any())
            {
                applicants = model.applicants.Select(x => x.UID).ToList();
                sql += "  and f.ApplicantID in @in_Applicants ";
                _params.Add("@in_Applicants", applicants);
            }


            sql += "  and FN_FormAuth(@in_UID, f.FormID) > 0 ";



            return (await AsyncDB.QueryAsync<FormInfoListModel>(str_conn, sql, _params)).ToList();
        }
        /// <summary>
        /// 取得表單結果
        /// </summary>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<SignResultTypeModel> GetFormResult(string FormID)
        {
            string sql = "select r.* from TB_Form_List f join TB_Sign_ResultType r on f.ResultID = r.ResultID where FormID = @in_FormID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", FormID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<SignResultTypeModel>(str_conn, sql, _params);
        }

        #region FormAuth Setting
        /// <summary>
        /// 取得啟用的表單權限
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public async Task<List<FormAuthModel>> GetFormAuthSetting(string UID)
        {
            string sql = "select * from TB_Form_Auth where UID=@in_UID and Status=1 ";
            
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_UID", UID, DbType.String);

            return await AsyncDB.QueryAsync<FormAuthModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得表單權限類型
        /// </summary>
        /// <returns></returns>
        public async Task<List<FormAuthTypeModel>> GetFormAuthType()
        {
            string sql = "select * from TB_Form_AuthType order by AuthType";
            return await AsyncDB.QueryAsync<FormAuthTypeModel>(str_conn, sql);
        }
        /// <summary>
        /// 修改表單權限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> EditFormAuth(string uid, FormAuthEditModel model)
        {
            int result = 0;

            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = "select exists(select 1 from TB_Form_Auth where UID=@in_UID and FormClass=@in_FormClass and AuthType=@in_AuthType )";
                        
                        DynamicParameters _params = new DynamicParameters();
                        _params.Add("@in_Editor", uid, DbType.String);
                        _params.Add("@in_UID", model.UID, DbType.String);
                        _params.Add("@in_FormClass", model.FormClass, DbType.String);
                        _params.Add("@in_AuthType", model.AuthType, DbType.String);
                        _params.Add("@in_Status", model.Status, DbType.Int16);

                        bool exists = await conn.QueryFirstOrDefaultAsync<bool>(sql, _params, transaction);


                        if (exists)
                        {
                            // update
                            sql = "UPDATE `TB_Form_Auth`" +
                                " SET "+
                                " `Status` = @in_Status , " +
                                " `EditDate` = now() , " +
                                " `Editor` = @in_Editor " +
                                "  WHERE `UID` = @in_UID AND `FormClass` = @in_FormClass AND `AuthType` = @in_AuthType; ";

                            result = await conn.ExecuteAsync(sql, _params, transaction);
                        }
                        else
                        {
                            // insert
                            sql = "INSERT INTO `TB_Form_Auth` (`UID`,`FormClass`,`AuthType`,`Status`,`EditDate`,`Editor`)" +
                                   " VALUES(@in_UID, @in_FormClass, @in_AuthType, @in_Status, now(), @in_Editor ); ";

                            result = await conn.ExecuteAsync(sql, _params, transaction);
                        }



                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 檢查表單權限 FormClass 修改狀態
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<bool> CheckAuthFormClassEditResult(string UID, string FormID)
        {
            string sql = "select exists ( " +
                         " select 1 from TB_Form_Auth a join TB_Form_List f on a.FormClass=f.FormClass" +
                         " where a.UID = @in_UID and a.AuthType = 2 and a.Status = 1 " +
                         " and f.FormID = @in_FormID and f.Status = 1 and f.ResultID is null" +
                         " )";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_UID", UID, DbType.String);
            _params.Add("@in_FormID", FormID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<bool>(str_conn, sql, _params);
        }
        #endregion

    }
}
