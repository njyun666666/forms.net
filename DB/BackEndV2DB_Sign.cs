using FormsNet.DB.DBClass;
using FormsNet.DB.IDB;
using FormsNet.Enums;
using FormsNet.Models.Flow;
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
    public class BackEndV2DB_Sign : IBackEndV2DB_Sign
    {
        public readonly string DbName = "BackEndV2DB";
        public string str_conn;

        public IDBConnection _dBConnection;

        IBackEndV2DB_File db_File;

        public BackEndV2DB_Sign(IDBConnection dBConnection, IBackEndV2DB_File backEndV2DB_File)
        {
            _dBConnection = dBConnection;
            str_conn = _dBConnection.Connection(DbName);
            db_File = backEndV2DB_File;
        }
        /// <summary>
        /// 取得此步驟 簽核記錄
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="SSID"></param>
        /// <returns></returns>
        public async Task<List<SignLogModel>> GetSignLogList(MySqlConnection conn, MySqlTransaction transaction, Int64 SSID)
        {
            string sql = "select * from TB_Sign_Log where SSID=@in_SSID for update";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_SSID", SSID, DbType.Int64);

            return (await conn.QueryAsync<SignLogModel>(sql, _params, transaction)).ToList();
        }
        /// <summary>
        /// 寫入簽核人清單
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="FormID"></param>
        /// <param name="SSID"></param>
        /// <param name="List"></param>
        /// <param name="nowStep"></param>
        /// <returns></returns>
        public async Task<int> AddSignLog(MySqlConnection conn, MySqlTransaction transaction, string FormID, Int64? SSID, List<FlowApproverModel> List, FlowStepModel nowStep, SignFormModel signModel = null, Int16 SSIDFinish = 0)
        {
            int result = 0;
            DateTime now = DateTime.Now;


            foreach (var item in List)
            {
                // 沒有DeptID的話，就抓一個DeptID
                if (string.IsNullOrWhiteSpace(item.DeptID))
                {
                    string sql_dept = "select DeptID from TB_Org_Account_Struct where UID=@in_UID and Status=1 order by Main desc ";
                    DynamicParameters _params_dept = new DynamicParameters();
                    _params_dept.Add("@in_UID", item.UID, DbType.String);
                    item.DeptID = await conn.QueryFirstOrDefaultAsync<string>(sql_dept, _params_dept, transaction);
                }


                string sql = "INSERT INTO `TB_Sign_Log` " +
                    "(`SSID`,`FormID`,`StepID`,`StepName`,`ApproverID`,`Approver`,`DeptID`,`DeptName`,`TitleID`,`Title`, `SignOptionID`,`SignOption`,`SignResultID`,`SignContent`,`SignDate`,`Status`,`ArrivalDate`,`FileGroupID`, `SSIDFinish`)" +
                    " select @in_SSID, @in_FormID, @in_StepID, @in_StepName," +
                    "  i.UID, i.Name, di.DeptID, di.DeptName, t.TitleID, t.Title," +
                    "  @in_SignOptionID, @in_SignOption, @in_SignResultID, @in_SignContent, @in_SignDate, @in_Status," +
                    "  @in_now, @in_FileGroupID, @in_SSIDFinish" +
                    "  from " +
                    " TB_Org_Account_Info i " +
                    " join TB_Org_Account_Struct s on i.UID = s.UID" +
                    " join TB_Org_Dept_Info di on s.DeptID = di.DeptID" +
                    " join TB_Org_Title t on s.TitleID = t.TitleID" +
                    " where i.UID=@in_UID and s.DeptID = @in_DeptID ";

                DynamicParameters _params = new DynamicParameters();
                _params.Add("@in_FormID", FormID, DbType.String);
                _params.Add("@in_SSID", SSID, DbType.Int64);
                _params.Add("@in_StepID", nowStep != null ? nowStep.StepID : null, DbType.Int32);
                _params.Add("@in_StepName", nowStep != null ? nowStep.StepName : null, DbType.String);
                _params.Add("@in_now", now, DbType.DateTime);
                _params.Add("@in_FileGroupID", Guid.NewGuid().ToString(), DbType.String);
                _params.Add("@in_UID", item.UID, DbType.String);
                _params.Add("@in_DeptID", item.DeptID, DbType.String);
                _params.Add("@in_SSIDFinish", SSIDFinish, DbType.Int16);

                // 寫入簽核結果
                _params.Add("@in_SignOptionID", signModel != null ? signModel.SignOptionID : null, DbType.Int32);
                _params.Add("@in_SignOption", signModel != null ? signModel.SignOption : null, DbType.String);
                _params.Add("@in_SignResultID", signModel != null ? signModel.SignResultID : null, DbType.Int32);
                _params.Add("@in_SignContent", signModel != null ? signModel.SignContent : null, DbType.String);
                _params.Add("@in_SignDate", signModel != null ? now : null, DbType.DateTime);
                _params.Add("@in_Status", signModel != null ? signModel.Status : 0, DbType.Int16);


                result += await conn.ExecuteAsync(sql, _params, transaction);
            }

            return result;
        }
        /// <summary>
        /// 取得簽核資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SignFormModel> GetSignForm(Int64 id)
        {
            string sql = "select * from TB_Sign_Log where ID=@in_id";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_id", id, DbType.Int64);

            return await AsyncDB.QueryFirstOrDefaultAsync<SignFormModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得簽核選項
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public async Task<List<SignOptionModel>> GetSignOption(string uid, GetSignOptionModel model)
        {
            string sql = "";


            switch (model.TypeID)
            {
                case SignOptionTypeEnum.applicant:
                    break;

                case SignOptionTypeEnum.sign:
                    sql = "select * from TB_Sign_Option where Status=1 and ( TypeID=@in_TypeID or ID in (select OptionID from TB_Sign_Option_Step where StepID=@in_StepID and Status=1)  ) order by Sort ";
                    break;

                case SignOptionTypeEnum.sign_other:
                    break;

                case SignOptionTypeEnum.applicant_edit:
                    sql = "select * from TB_Sign_Option where Status=1 and TypeID=4 and exists (" +
                            " select 1 from TB_Form_List where FormID=@in_FormID and ApplicantID=@in_UID and ResultID is null" +
                          " )" +
                          " union" +
                          " select * from TB_Sign_Option" +
                          " where Status=1 and TypeID=5 " +
                          " and exists ( " +
                          "     select 1 from TB_Form_Auth a join TB_Form_List f on a.FormClass=f.FormClass" +
                          "     where a.UID=@in_UID and a.AuthType=2 and a.Status=1 " +
                          "     and f.FormID=@in_FormID and f.Status=1 and f.ResultID is null" +
                          " )";
                    break;
                default:
                    break;
            }


            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_TypeID", model.TypeID, DbType.Int32);
            _params.Add("@in_StepID", model.StepID, DbType.Int32);
            _params.Add("@in_FormID", model.FormID, DbType.String);
            _params.Add("@in_UID", uid, DbType.String);

            return await AsyncDB.QueryAsync<SignOptionModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 簽核
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="signModel"></param>
        /// <returns></returns>
        public async Task<int> Sign(MySqlConnection conn, MySqlTransaction transaction, string uid, SignModel signModel)
        {
            DateTime now = DateTime.Now;

            string sql = "UPDATE `TB_Sign_Log`" +
                        " SET" +
                        " `SignOptionID` = @in_SignOptionID ," +
                        " `SignOption` = @in_SignOption ," +
                        " `SignResultID` = @in_SignResultID ," +
                        " `SignContent` = @in_SignContent," +
                        " `SignDate` = @in_now ," +
                        " `Status` = @in_Status ," +
                        " `UpdateDate` = @in_now " +
                        "  WHERE `ID` = @in_ID ; ";


            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_SignOptionID", signModel.SignOptionID, DbType.Int32);
            _params.Add("@in_SignOption", signModel.SignOption, DbType.String);
            _params.Add("@in_SignResultID", signModel.SignResultID, DbType.Int32);
            _params.Add("@in_SignContent", signModel.SignContent, DbType.String);
            _params.Add("@in_now", now, DbType.DateTime);
            _params.Add("@in_Status", signModel.Status, DbType.Int16);
            _params.Add("@in_ID", signModel.ID, DbType.Int64);

            
            int main = await conn.ExecuteAsync(sql, _params, transaction);

            if (main == 0)
            {
                return main;
            }

            // 附件轉正式
            await db_File.FileToRelease(conn, transaction, uid, signModel.FileGroupID, now);

            return 1;

        }
        /// <summary>
        /// 設定簽核Log狀態
        /// </summary>
        /// <param name="SignID"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public async Task<int> SetStatus(Int64 SignID, int Status)
        {
            string sql = "update TB_Sign_Log set Status=@in_Status where ID=@in_ID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_ID", SignID, DbType.Int64);
            _params.Add("@in_Status", Status, DbType.Int32);

            return await AsyncDB.ExecuteAsync(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得簽核紀錄
        /// </summary>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<List<SignLogViewModel>> SignLogList(string FormID)
        {
            string sql = "select StepName, Approver, DeptName, SignOption, SignResultID, SignContent, SignDate from TB_Sign_Log " +
                        " where FormID = @in_FormID and Status = 1 ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", FormID, DbType.String);

            return (await AsyncDB.QueryAsync<SignLogViewModel>(str_conn, sql, _params)).ToList();
        }
        /// <summary>
        /// 取得目前步驟/簽核人
        /// </summary>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<List<NowStepApproverModel>> NowStepApprover(string FormID)
        {
            string sql = "select SSID, StepName, Approver from TB_Sign_Log where FormID=@in_FormID and SSIDFinish=0 ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", FormID, DbType.String);

            return (await AsyncDB.QueryAsync<NowStepApproverModel>(str_conn, sql, _params)).ToList();
        }
        

    }
}
