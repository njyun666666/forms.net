using FormsNet.DB.DBClass;
using FormsNet.DB.IDB;
using FormsNet.Models.Flow;
using FormsNet.Models.Form;
using FormsNet.Models.Form.SR;
using FormsNet.Models.Sign;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB.Form
{
    public class DB_Form_SR : IDB_Form_SR
    {
        public readonly string DbName = "BackEndV2DB";
        public string str_conn;

        public IDBConnection _dBConnection;
        IBackEndV2DB_Form db_Form;
        IBackEndV2DB_Sign db_Sign;

        public DB_Form_SR(IDBConnection dBConnection, IBackEndV2DB_Form backEndV2DB_Form, IBackEndV2DB_Sign backEndV2DB_Sign)
        {
            _dBConnection = dBConnection;
            str_conn = _dBConnection.Connection(DbName);
            db_Form = backEndV2DB_Form;
            db_Sign = backEndV2DB_Sign;
        }
        public async Task<SRModel> GetFormData(string formID)
        {
            string sql = "select * from TB_Form_List l join TB_Form_SR sr on l.FormID = sr.FormID where l.FormID = @in_formID ";
            
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_formID", formID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<SRModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得承辦人清單
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<List<SRTaskOwnerModel>> GeTaskOwner(string formID)
        {
            string sql = "select * from TB_Form_SR_TaskOwner where FormID = @in_formID order by ID";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_formID", formID, DbType.String);

            return await AsyncDB.QueryAsync<SRTaskOwnerModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 儲存草稿/申請表單 儲存表單資料
        /// </summary>
        /// <param name="listModel"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Applicant(FormListModel listModel, SRModel model)
        {
            int result = 0;

            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {

                        // 先寫主表
                        int mainResult = await db_Form.Applicant(conn, transaction, listModel);

                        if (mainResult == 0)
                        {
                            await transaction.RollbackAsync();
                            return 0;
                        }

                        #region 表單專屬主表/資料表

                        // SR
                        string sql = "DELETE FROM `TB_Form_SR` WHERE FormID = (select FormID from TB_Form_List where FormID = @in_FormID and FillerID = @in_FillerID ) ; ";

                        sql += " INSERT INTO `TB_Form_SR`" +
                            " (`FormID`,`Type`,`Other`,`Subject`,`Content`,`HavePhone`,`ExpectedDate`)" +
                            " VALUES" +
                            " (@in_FormID, @in_Type, @in_Other, @in_Subject, @in_Content, @in_HavePhone, @in_ExpectedDate );";

                        DynamicParameters _params = new DynamicParameters();
                        _params.Add("@in_FillerID", listModel.FillerID, DbType.String, size: 255);
                        _params.Add("@in_FormID", model.FormID, DbType.String, size: 255);
                        _params.Add("@in_Type", model.Type, DbType.Int32);
                        _params.Add("@in_Other", model.Other, DbType.String, size: 255);
                        _params.Add("@in_Subject", model.Subject, DbType.String, size: 255);
                        _params.Add("@in_Content", model.Content, DbType.String, size: 2000);
                        _params.Add("@in_HavePhone", model.HavePhone, DbType.Int32);
                        _params.Add("@in_ExpectedDate", model.ExpectedDate, DbType.DateTime);

                        result = await conn.ExecuteAsync(sql, _params, transaction);

                        #endregion


                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }

            }

            return result;
        }
        /// <summary>
        /// 簽核 儲存簽核log/表單資料
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="signModel"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Sign(string uid, SignModel signModel, SRModel model)
        {
            int result = 0;

            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        DynamicParameters _params = new DynamicParameters();
                        string sql = "";

                        // 先寫簽核Log
                        int signLogResult = await db_Sign.Sign(conn, transaction, uid, signModel);

                        if (signLogResult == 0)
                        {
                            await transaction.RollbackAsync();
                            return 0;
                        }



                        #region 依步驟更新表單資料
                        if (model != null)
                        {


                            // 技術方主管分派任務
                            if (signModel.StepID == 8)
                            {
                                // 新增承辦人
                                var insertList = model.taskOwnerList.Where(x => !x.ID.HasValue).ToList();

                                foreach (var owner in insertList)
                                {
                                    sql = "INSERT INTO `TB_Form_SR_TaskOwner` (`FormID`,`UID`,`Name`,`WorkDay`,`TestDay`,`EstStartDate`,`EstEndDate`,`RealStartDate`)" +
                                        " VALUES (@in_FormID, @in_UID, @in_Name, @in_WorkDay, @in_TestDay, @in_EstStartDate, @in_EstEndDate, @in_RealStartDate );";

                                    _params = new DynamicParameters();
                                    _params.Add("@in_FormID", model.FormID, DbType.String);
                                    _params.Add("@in_UID", owner.UID, DbType.String);
                                    _params.Add("@in_Name", owner.Name, DbType.String);
                                    _params.Add("@in_WorkDay", owner.WorkDay, DbType.Int32);
                                    _params.Add("@in_TestDay", owner.TestDay, DbType.Int32);
                                    _params.Add("@in_EstStartDate", owner.EstStartDate, DbType.DateTime);
                                    _params.Add("@in_EstEndDate", owner.EstEndDate, DbType.DateTime);
                                    _params.Add("@in_RealStartDate", owner.RealStartDate, DbType.DateTime);

                                    await conn.ExecuteAsync(sql, _params, transaction);
                                }


                                // 更新承辦人
                                var updateList = model.taskOwnerList.Where(x => x.ID.HasValue).ToList();

                                foreach (var owner in updateList)
                                {
                                    sql = "UPDATE `TB_Form_SR_TaskOwner`" +
                                        " SET" +
                                        " `UID` = @in_UID, " +
                                        " `Name` = @in_Name, " +
                                        " `WorkDay` = @in_WorkDay, " +
                                        " `TestDay` = @in_TestDay, " +
                                        " `EstStartDate` = @in_EstStartDate, " +
                                        " `EstEndDate` = @in_EstEndDate, " +
                                        " `RealStartDate` = @in_RealStartDate " +
                                        " WHERE `ID` = @in_ID and `FormID`= @in_FormID ; ";

                                    _params = new DynamicParameters();
                                    _params.Add("@in_FormID", model.FormID, DbType.String);
                                    _params.Add("@in_ID", owner.ID, DbType.Int32);
                                    _params.Add("@in_UID", owner.UID, DbType.String);
                                    _params.Add("@in_Name", owner.Name, DbType.String);
                                    _params.Add("@in_WorkDay", owner.WorkDay, DbType.Int32);
                                    _params.Add("@in_TestDay", owner.TestDay, DbType.Int32);
                                    _params.Add("@in_EstStartDate", owner.EstStartDate, DbType.DateTime);
                                    _params.Add("@in_EstEndDate", owner.EstEndDate, DbType.DateTime);
                                    _params.Add("@in_RealStartDate", owner.RealStartDate, DbType.DateTime);

                                    await conn.ExecuteAsync(sql, _params, transaction);
                                }


                            }
                            // 技術開發
                            else if (signModel.StepID == 7)
                            {
                                var owner = model.taskOwnerList.Where(x => x.UID == uid).FirstOrDefault();

                                if (owner != null)
                                {
                                    sql = "UPDATE `TB_Form_SR_TaskOwner`" +
                                            " SET" +
                                            //" `UID` = @in_UID, " +
                                            //" `Name` = @in_Name, " +
                                            " `WorkDay` = @in_WorkDay, " +
                                            " `TestDay` = @in_TestDay, " +
                                            " `EstStartDate` = @in_EstStartDate, " +
                                            " `EstEndDate` = @in_EstEndDate, " +
                                            " `RealStartDate` = @in_RealStartDate " +
                                            " WHERE `ID` = @in_ID and `FormID`= @in_FormID and `UID`=@in_UID ; ";

                                    _params = new DynamicParameters();
                                    _params.Add("@in_FormID", model.FormID, DbType.String);
                                    _params.Add("@in_ID", owner.ID, DbType.Int32);
                                    _params.Add("@in_UID", uid, DbType.String);
                                    //_params.Add("@in_Name", owner.Name, DbType.String);
                                    _params.Add("@in_WorkDay", owner.WorkDay, DbType.Int32);
                                    _params.Add("@in_TestDay", owner.TestDay, DbType.Int32);
                                    _params.Add("@in_EstStartDate", owner.EstStartDate, DbType.DateTime);
                                    _params.Add("@in_EstEndDate", owner.EstEndDate, DbType.DateTime);
                                    _params.Add("@in_RealStartDate", owner.RealStartDate, DbType.DateTime);

                                    await conn.ExecuteAsync(sql, _params, transaction);
                                }
                            }
                            // 技術主管確認功能開發完成
                            else if (signModel.StepID == 11)
                            {
                                // 新增承辦人
                                var insertList = model.taskOwnerList.Where(x => !x.ID.HasValue).ToList();

                                foreach (var owner in insertList)
                                {
                                    sql = "INSERT INTO `TB_Form_SR_TaskOwner` (`FormID`,`UID`,`Name`,`WorkDay`,`TestDay`,`EstStartDate`,`EstEndDate`,`RealStartDate`)" +
                                        " VALUES (@in_FormID, @in_UID, @in_Name, @in_WorkDay, @in_TestDay, @in_EstStartDate, @in_EstEndDate, @in_RealStartDate );";

                                    _params = new DynamicParameters();
                                    _params.Add("@in_FormID", model.FormID, DbType.String);
                                    _params.Add("@in_UID", owner.UID, DbType.String);
                                    _params.Add("@in_Name", owner.Name, DbType.String);
                                    _params.Add("@in_WorkDay", owner.WorkDay, DbType.Int32);
                                    _params.Add("@in_TestDay", owner.TestDay, DbType.Int32);
                                    _params.Add("@in_EstStartDate", owner.EstStartDate, DbType.DateTime);
                                    _params.Add("@in_EstEndDate", owner.EstEndDate, DbType.DateTime);
                                    _params.Add("@in_RealStartDate", owner.RealStartDate, DbType.DateTime);

                                    await conn.ExecuteAsync(sql, _params, transaction);
                                }


                                // 更新承辦人
                                var updateList = model.taskOwnerList.Where(x => x.ID.HasValue).ToList();

                                foreach (var owner in updateList)
                                {
                                    sql = "UPDATE `TB_Form_SR_TaskOwner`" +
                                        " SET" +
                                        " `UID` = @in_UID, " +
                                        " `Name` = @in_Name, " +
                                        " `WorkDay` = @in_WorkDay, " +
                                        " `TestDay` = @in_TestDay, " +
                                        " `EstStartDate` = @in_EstStartDate, " +
                                        " `EstEndDate` = @in_EstEndDate, " +
                                        " `RealStartDate` = @in_RealStartDate " +
                                        " WHERE `ID` = @in_ID and `FormID`= @in_FormID ; ";

                                    _params = new DynamicParameters();
                                    _params.Add("@in_FormID", model.FormID, DbType.String);
                                    _params.Add("@in_ID", owner.ID, DbType.Int32);
                                    _params.Add("@in_UID", owner.UID, DbType.String);
                                    _params.Add("@in_Name", owner.Name, DbType.String);
                                    _params.Add("@in_WorkDay", owner.WorkDay, DbType.Int32);
                                    _params.Add("@in_TestDay", owner.TestDay, DbType.Int32);
                                    _params.Add("@in_EstStartDate", owner.EstStartDate, DbType.DateTime);
                                    _params.Add("@in_EstEndDate", owner.EstEndDate, DbType.DateTime);
                                    _params.Add("@in_RealStartDate", owner.RealStartDate, DbType.DateTime);

                                    await conn.ExecuteAsync(sql, _params, transaction);
                                }


                            }
                            // 需求方安排QA測試並驗收
                            else if (signModel.StepID == 4)
                            {
                                sql = " UPDATE `TB_Form_SR`" +
                                        " SET" +
                                        " `OnlineDate` = @in_OnlineDate " +
                                        "  WHERE `FormID` = @in_FormID ; ";

                                _params = new DynamicParameters();
                                _params.Add("@in_FormID", model.FormID, DbType.String);
                                _params.Add("@in_OnlineDate", model.OnlineDate, DbType.DateTime);

                                await conn.ExecuteAsync(sql, _params, transaction);
                            }



                        }
                        #endregion

                        result = 1;
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }

            }

            return result;
        }
        /// <summary>
        /// 取得承辦人清單
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<List<FlowApproverModel>> SRTaskOwner(MySqlConnection conn, MySqlTransaction transaction, string FormID)
        {
            string sql = "select o.UID, s.DeptID from TB_Form_SR_TaskOwner o join TB_Org_Account_Struct s on o.UID=s.UID" +
                         " where o.FormID = @in_FormID and s.Main = 1 and s.Status = 1";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", FormID);

            return (await conn.QueryAsync<FlowApproverModel>(sql, _params, transaction)).ToList();
        }

    }
}
