using FormsNet.DB.DBClass;
using FormsNet.DB.IDB;
using FormsNet.DB.IDB.Form;
using FormsNet.Models.Form;
using FormsNet.Models.Form.DEMO;
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
    public class DB_Form_DEMO : IDB_Form_DEMO
    {
        public readonly string DbName = "BackEndV2DB";
        public string str_conn;

        public IDBConnection _dBConnection;
        IBackEndV2DB_Form db_Form;
        IBackEndV2DB_Sign db_Sign;


        public DB_Form_DEMO(IDBConnection dBConnection, IBackEndV2DB_Form backEndV2DB_Form, IBackEndV2DB_Sign backEndV2DB_Sign)
        {
            _dBConnection = dBConnection;
            str_conn = _dBConnection.Connection(DbName);
            db_Form = backEndV2DB_Form;
            db_Sign = backEndV2DB_Sign;
        }
        /// <summary>
        /// 取得表單資料
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<DEMOModel> GetFormData(string formID)
        {
            string sql = "select * from TB_Form_List l join TB_Form_DEMO d on l.FormID = d.FormID where l.FormID = @in_formID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_formID", formID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<DEMOModel>(str_conn, sql, _params);
        }

        /// <summary>
        /// 儲存草稿/申請表單 儲存表單資料
        /// </summary>
        /// <param name="listModel"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Applicant(FormListModel listModel, DEMOModel model)
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

                        // DEMO
                        string sql = "DELETE FROM `TB_Form_DEMO` WHERE FormID = (select FormID from TB_Form_List where FormID = @in_FormID and FillerID = @in_FillerID ) ; ";

                        sql += " INSERT INTO `TB_Form_DEMO`" +
                            " (`FormID`,`AAA`,`BBB`)" +
                            " VALUES" +
                            " (@in_FormID, @in_AAA, @in_BBB);";

                        DynamicParameters _params = new DynamicParameters();
                        _params.Add("@in_FillerID", listModel.FillerID, DbType.String);
                        _params.Add("@in_FormID", model.FormID, DbType.String);
                        _params.Add("@in_AAA", model.AAA, DbType.String);
                        _params.Add("@in_BBB", model.BBB, DbType.String);

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
        public async Task<int> Sign(string uid, SignModel signModel, DEMOModel model)
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
                            // 申請人
                            if (signModel.StepID == 13)
                            {
                                sql = "UPDATE `TB_Form_DEMO`" +
                                     " SET" +
                                     " `BBB` = @in_BBB " +
                                     "  WHERE `FormID` = @in_FormID ;";

                                _params = new DynamicParameters();
                                _params.Add("@in_FormID", model.FormID, DbType.String);
                                _params.Add("@in_BBB", model.BBB, DbType.String);

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

    }
}
