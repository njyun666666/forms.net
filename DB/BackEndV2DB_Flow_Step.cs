using FormsNet.DB.DBClass;
using FormsNet.DB.IDB;
using FormsNet.Enums;
using FormsNet.Models.Flow;
using FormsNet.Models.Form;
using FormsNet.Models.Organization;
using FormsNet.Models.Sign;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormsNet.DB
{
    public class BackEndV2DB_Flow_Step : IBackEndV2DB_Flow_Step
    {
        public readonly string DbName = "BackEndV2DB";
        public string str_conn;

        public IDBConnection _dBConnection;
        IDB_Form_SR db_SR;


        public BackEndV2DB_Flow_Step(IDBConnection dBConnection, IDB_Form_SR dB_Form_SR)
        {
            _dBConnection = dBConnection;
            db_SR = dB_Form_SR;
            str_conn = _dBConnection.Connection(DbName);
        }

        #region 

        /// <summary>
        /// 取得表單主表資料
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<FormListModel> FormData(MySqlConnection conn, MySqlTransaction transaction, string FormID)
        {
            string sql = "select * from TB_Form_List where FormID=@in_FormID";
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", FormID, DbType.String);
            return await conn.QueryFirstOrDefaultAsync<FormListModel>(sql, _params, transaction);
        }
        /// <summary>
        /// 設定SSIS步驟結束
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="SSID"></param>
        /// <param name="SSIDFinish"></param>
        /// <returns></returns>
        public async Task<FormListModel> SetSSIDFinish(MySqlConnection conn, MySqlTransaction transaction, Int64 SSID, int SSIDFinish)
        {
            string sql = "UPDATE `TB_Sign_Log`" +
                        " SET" +
                        " `SSIDFinish` = @in_SSIDFinish " +
                        "  WHERE `SSID` = @in_SSID; ";
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_SSID", SSID, DbType.Int64);
            _params.Add("@in_SSIDFinish", SSIDFinish, DbType.Int32);

            return await conn.QueryFirstOrDefaultAsync<FormListModel>(sql, _params, transaction);
        }
        /// <summary>
        /// 找部門簽核人
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="UID"></param>
        /// <param name="DeptID"></param>
        /// <param name="nowStep"></param>
        /// <param name="FindParentDept">往上層部門找</param>
        /// <returns>簽核人清單</returns>
        public async Task<List<FlowApproverModel>> FindDeptSignApprover(MySqlConnection conn, MySqlTransaction transaction, string UID, string DeptID, FlowStepModel nowStep, bool FindParentDept = false)
        {
            List<FlowApproverModel> list = null;

            if (!FindParentDept)
            {
                string sql = "select UID, DeptID from TB_Org_Account_Struct where DeptID=@in_DeptID and Status=1 and SignApprover=1 ";
                DynamicParameters _params = new DynamicParameters();
                _params.Add("@in_DeptID", DeptID, DbType.String);
                list = (await conn.QueryAsync<FlowApproverModel>(sql, _params, transaction)).ToList();
            }


            // 找不到部門簽核人
            if (nowStep.CanFindParentDept == 1 && (list == null || !list.Any() || (nowStep.WhenSelfSign == FlowStepWhenTypeEnum.find_parent.ToString() && list.Where(x => x.UID == UID).Any())))
            {
                // 找上層部門
                string sql_parentDept = "select ParentDept from TB_Org_Dept_Struct where DeptID=@in_DeptID and Status=1 ";
                DynamicParameters _params_parentDept = new DynamicParameters();
                _params_parentDept.Add("@in_DeptID", DeptID, DbType.String);
                string parentDept = await conn.QueryFirstOrDefaultAsync<string>(sql_parentDept, _params_parentDept, transaction);

                if (!string.IsNullOrWhiteSpace(parentDept))
                {
                    list = await FindDeptSignApprover(conn, transaction, UID, parentDept, nowStep);
                }
            }

            

            return list;
        }
        /// <summary>
        /// 找部門層級簽核人
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="UID"></param>
        /// <param name="DeptID"></param>
        /// <param name="nowStep"></param>
        /// <returns>簽核人清單</returns>
        public async Task<List<FlowApproverModel>> FindDeptLevelSignApprover(MySqlConnection conn, MySqlTransaction transaction, string UID, string DeptID, FlowStepModel nowStep)
        {
            List<FlowApproverModel> list = null;
            int DeptLevel = Convert.ToInt32(nowStep.Value);


            string sql = " select DeptID, ParentDept, l.Level from TB_Org_Dept_Struct s join TB_Org_Dept_Level l on s.DeptLevelID=l.DeptLevelID" +
                            " where s.DeptID = @in_DeptID and s.Status = 1 ";
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_DeptID", DeptID, DbType.String);
            OrgDeptParentModel dept = await conn.QueryFirstOrDefaultAsync<OrgDeptParentModel>(sql, _params, transaction);

            // 找到目標層級
            if (dept != null && dept.Level >= DeptLevel)
            {
                list = await FindDeptSignApprover(conn, transaction, UID, dept.DeptID, nowStep);
            }
            // 往上層找
            else if (dept != null && dept.Level < DeptLevel && !string.IsNullOrWhiteSpace(dept.ParentDept))
            {
                list = await FindDeptLevelSignApprover(conn, transaction, UID, dept.ParentDept, nowStep);
            }

            return list;
        }
        /// <summary>
        /// 核決條件
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ssmodel"></param>
        /// <param name="nowStepSignLog"></param>
        /// <returns></returns>
        public async Task<StepValueResponseModel> SignCountCheck(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog)
        {
            StepValueResponseModel result = new StepValueResponseModel();


            // 判斷簽核條件
            // 一人簽核
            if (ssmodel.SignCountType == (int)SignCountTypeEnum.one_sign)
            {
                if (nowStepSignLog.Where(x => x.Status == 1).Any())
                {
                    result.StepAllComplete = true;
                    result.SignResult = nowStepSignLog.Where(x => x.Status == 1).FirstOrDefault().SignResultID;

                    await SetSSIDFinish(conn, transaction, ssmodel.SSID, 1);
                }
            }
            else
            {
                // 比例決

                decimal canSignCount = nowStepSignLog.Count();
                decimal agreeCount = nowStepSignLog.Where(x => x.SignResultID == SignResultTypeEnum.agree).Count();
                decimal rejectCount = nowStepSignLog.Where(x => x.SignResultID == SignResultTypeEnum.reject).Count();

                decimal agreeRatio = Math.Round(agreeCount / canSignCount, 2) * 100;
                decimal rejectRatio = Math.Round(rejectCount / canSignCount, 2) * 100;
                decimal notyetRatio = Math.Round(100 - agreeRatio - rejectRatio, 2);

                // 同意比例達標
                if (agreeRatio >= ssmodel.AgreeRatio)
                {
                    result.StepAllComplete = true;
                    result.SignResult = SignResultTypeEnum.agree;
                    await SetSSIDFinish(conn, transaction, ssmodel.SSID, 1);
                }
                // 駁回比例達標
                else if (rejectRatio >= ssmodel.AgreeRatio)
                {
                    result.StepAllComplete = true;
                    result.SignResult = SignResultTypeEnum.reject;
                    await SetSSIDFinish(conn, transaction, ssmodel.SSID, 1);
                }
                // 同意+未簽核數，無法改變結果時，此步驟駁回
                else if ((agreeRatio + notyetRatio) < ssmodel.AgreeRatio)
                {
                    result.StepAllComplete = true;
                    result.SignResult = SignResultTypeEnum.reject;
                    await SetSSIDFinish(conn, transaction, ssmodel.SSID, 1);
                }



            }

            return result;
        }
        /// <summary>
        /// 取得部門層級
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="DeptID"></param>
        /// <returns></returns>
        public async Task<int> GetDeptLevel(MySqlConnection conn, MySqlTransaction transaction, string DeptID)
        {
            string sql = "select l.Level from TB_Org_Dept_Struct s join TB_Org_Dept_Level l on s.DeptLevelID=l.DeptLevelID where s.DeptID = @in_DeptID ";
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_DeptID", DeptID, DbType.String);
            return await conn.QueryFirstOrDefaultAsync<int>(sql, _params, transaction);
        }
        public async Task<FlowStepSystemLogModel> GetPrevFlowStepSystemNeedSign(MySqlConnection conn, MySqlTransaction transaction, Int64 NextSSID)
        {
            string sql = "select * from TB_Flow_Step_SystemLog where NextSSID=@in_NextSSID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_NextSSID", NextSSID, DbType.Int64);

            FlowStepSystemLogModel model = await conn.QueryFirstOrDefaultAsync<FlowStepSystemLogModel>(sql, _params, transaction);

            if (model != null && model.NeedSign == 0)
            {
                model = await GetPrevFlowStepSystemNeedSign(conn, transaction, model.SSID);
            }

            return model;
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
            string sql = "select * from TB_Sign_Log where SSID=@in_SSID";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_SSID", SSID, DbType.Int64);

            return (await conn.QueryAsync<SignLogModel>(sql, _params, transaction)).ToList();
        }
        public async Task<List<SignLogModel>> GetPrevSignLog(MySqlConnection conn, MySqlTransaction transaction, Int64 SSID)
        {
            // 取得上個SSID簽核步驟
            FlowStepSystemLogModel prevSSModel = await GetPrevFlowStepSystemNeedSign(conn, transaction, SSID);
            // 簽核記錄
            List<SignLogModel> prevStepSignLog = null;

            if (prevSSModel != null)
            {
                // 簽核記錄
                prevStepSignLog = await GetSignLogList(conn, transaction, prevSSModel.SSID);
            }

            return prevStepSignLog;
        }
        /// <summary>
        /// 檢查是否有簽核人
        /// </summary>
        /// <param name="result"></param>
        /// <param name="list"></param>
        /// <param name="nowStep"></param>
        /// <returns></returns>
        public async Task<StepValueResponseModel> CheckHaveApprover(StepValueResponseModel result, List<FlowApproverModel> list, FlowStepModel nowStep)
        {
            if (list != null && list.Any())
            {
                result.List = list;
            }
            else
            {

                if (nowStep.WhenNotFoundApprover == FlowStepWhenTypeEnum.exception.ToString())
                {
                    // 拋出例外
                    throw new Exception($"StepID={nowStep.StepID} 找不到簽核人");
                }
                else
                {
                    result.StepAllComplete = true;
                    result.SignResult = SignResultTypeEnum.agree;
                    result.Value = "找不到簽核人，略過簽核";
                }

            }

            return result;
        }
        #endregion

        #region ValueSetting
        public async Task<List<FlowApproverModel>> ValueSetting(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep)
        {
            if (ssmodel.ValueSetting == StepValueSettingEnum.ValueField.ToString())
            {
                return await ValueField(conn, transaction, ssmodel.ValueField, ssmodel.FormID);
            }
            else if (ssmodel.ValueSetting == StepValueSettingEnum.ValueFunction.ToString())
            {
                return await ValueFunction(conn, transaction, ssmodel.ValueFunction, ssmodel.FormID);
            }

            return null;
        }
        /// <summary>
        /// 目標指定欄位
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ValueField"></param>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<List<FlowApproverModel>> ValueField(MySqlConnection conn, MySqlTransaction transaction, string ValueField, string FormID)
        {
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", FormID, DbType.String);

            // 取FormClass
            string sql = "select FormClass from TB_Form_List where FormID=@in_FormID ";
            string FormClass = await conn.QueryFirstOrDefaultAsync<string>(sql, _params, transaction);

            if (string.IsNullOrWhiteSpace(FormClass))
            {
                return null;
            }

            // 表單資料 View表
            sql = $"select {ValueField} from View_Form_{FormClass} where FormID=@in_FormID";
            string jsonStr = await conn.QueryFirstOrDefaultAsync<string>(sql, _params, transaction);

            return JsonSerializer.Deserialize<List<FlowApproverModel>>(jsonStr);
        }
        /// <summary>
        /// 目標指定函式
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ValueFunction"></param>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<List<FlowApproverModel>> ValueFunction(MySqlConnection conn, MySqlTransaction transaction, string ValueFunction, string FormID)
        {
            switch (ValueFunction)
            {
                case "SRTaskOwner()":
                    return await db_SR.SRTaskOwner(conn, transaction, FormID);

                default:
                    break;
            }

            return null;
        }
        public async Task<bool> ValueSettingBoolean(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep)
        {
            if (ssmodel.ValueSetting == StepValueSettingEnum.ValueFieldBoolean.ToString())
            {
                return await ValueFieldBoolean(conn, transaction, nowStep.ValueFieldBoolean, ssmodel.FormID);
            }
            else if (ssmodel.ValueSetting == StepValueSettingEnum.ValueFunctionBoolean.ToString())
            {
                return await ValueFunctionBoolean(conn, transaction, nowStep.ValueFunctionBoolean, ssmodel.FormID, nowStep);
            }

            throw new Exception($"StepID={nowStep.StepID} 條件式找不到回傳值");
        }
        /// <summary>
        /// 目標指定Boolean欄位
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ValueField"></param>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<bool> ValueFieldBoolean(MySqlConnection conn, MySqlTransaction transaction, string ValueField, string FormID)
        {
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", FormID, DbType.String);

            // 取FormClass
            string sql = "select FormClass from TB_Form_List where FormID=@in_FormID ";
            string FormClass = await conn.QueryFirstOrDefaultAsync<string>(sql, _params, transaction);

            if (string.IsNullOrWhiteSpace(FormClass))
            {
                return false;
            }

            // 表單資料 View表
            sql = $"select {ValueField} from View_Form_{FormClass} where FormID=@in_FormID";
            return await conn.QueryFirstOrDefaultAsync<bool>(sql, _params, transaction);
        }
        /// <summary>
        /// 目標指定Boolean函式
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ValueFunctionBoolean"></param>
        /// <param name="FormID"></param>
        /// <param name="nowStep"></param>
        /// <returns></returns>
        public async Task<bool> ValueFunctionBoolean(MySqlConnection conn, MySqlTransaction transaction, string ValueFunctionBoolean, string FormID, FlowStepModel nowStep)
        {
            switch (ValueFunctionBoolean)
            {
                case "Test()":
                    return true;

                default:
                    break;
            }

            throw new Exception($"StepID={nowStep.StepID} 條件式找不到回傳值");
        }
        #endregion

        /// <summary>
        /// 申請人
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ssmodel"></param>
        /// <param name="nowStepSignLog"></param>
        /// <returns></returns>
        public async Task<StepValueResponseModel> ApplicantSelf(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep)
        {
            StepValueResponseModel result = new StepValueResponseModel();

            // 首次進入步驟
            if (nowStepSignLog == null || !nowStepSignLog.Any())
            {
                FormListModel formData = await FormData(conn, transaction, ssmodel.FormID);
                result.List.Add(new FlowApproverModel() { UID = formData.ApplicantID, DeptID = formData.ApplicantDeptID });
            }
            else
            {
                // 簽核結果判斷
                var log = nowStepSignLog.Where(x => x.Status == 1).FirstOrDefault();
                if (log != null)
                {
                    result.StepAllComplete = true;
                    result.SignResult = log.SignResultID;

                    await SetSSIDFinish(conn, transaction, ssmodel.SSID, 1);
                }
            }

            return result;
        }
        /// <summary>
        /// 部門主管
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ssmodel"></param>
        /// <param name="nowStepSignLog"></param>
        /// <returns></returns>
        public async Task<StepValueResponseModel> ApplicantDepartment(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep)
        {
            StepValueResponseModel result = new StepValueResponseModel();

            // 首次進入步驟
            if (nowStepSignLog == null || !nowStepSignLog.Any())
            {
                FormListModel formData = await FormData(conn, transaction, ssmodel.FormID);
                List<FlowApproverModel> list = await FindDeptSignApprover(conn, transaction, formData.ApplicantID, formData.ApplicantDeptID, nowStep);

                result = await CheckHaveApprover(result, list, nowStep);
            }
            else
            {
                result = await SignCountCheck(conn, transaction, ssmodel, nowStepSignLog);
            }

            return result;
        }
        /// <summary>
        /// 指定對象(會簽)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ssmodel"></param>
        /// <param name="nowStepSignLog"></param>
        /// <param name="nowStep"></param>
        /// <returns></returns>
        public async Task<StepValueResponseModel> DesignateList(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep)
        {
            StepValueResponseModel result = new StepValueResponseModel();

            // 首次進入步驟
            if (nowStepSignLog == null || !nowStepSignLog.Any())
            {
                //FormListModel formData = await FormData(conn, transaction, ssmodel.FormID);
                List<FlowApproverModel> list = null;


                if (ssmodel.ValueSetting == StepValueSettingEnum.Value.ToString())
                {
                    list = new List<FlowApproverModel>() { new FlowApproverModel() { UID = ssmodel.Value } };
                }
                else
                {
                    list = await ValueSetting(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                }

                result = await CheckHaveApprover(result, list, nowStep);

            }
            else
            {
                result = await SignCountCheck(conn, transaction, ssmodel, nowStepSignLog);
            }

            return result;
        }
        /// <summary>
        /// 指定部門(指定)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ssmodel"></param>
        /// <param name="nowStepSignLog"></param>
        /// <returns></returns>
        public async Task<StepValueResponseModel> GotoSpecificDepartment(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep)
        {
            StepValueResponseModel result = new StepValueResponseModel();

            // 首次進入步驟
            if (nowStepSignLog == null || !nowStepSignLog.Any())
            {
                //FormListModel formData = await FormData(conn, transaction, ssmodel.FormID);
                List<FlowApproverModel> list = null;

                if (ssmodel.ValueSetting == StepValueSettingEnum.Value.ToString())
                {
                    list = await FindDeptSignApprover(conn, transaction, null, ssmodel.Value, nowStep);
                }
                else
                {
                    list = await ValueSetting(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                }

                result = await CheckHaveApprover(result, list, nowStep);

            }
            else
            {
                result = await SignCountCheck(conn, transaction, ssmodel, nowStepSignLog);
            }


            return result;
        }
        /// <summary>
        /// 指定職稱主管(指定)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ssmodel"></param>
        /// <param name="nowStepSignLog"></param>
        /// <param name="nowStep"></param>
        /// <returns></returns>
        public async Task<StepValueResponseModel> GotoSpecificJobTitle(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep)
        {
            StepValueResponseModel result = new StepValueResponseModel();

            // 首次進入步驟
            if (nowStepSignLog == null || !nowStepSignLog.Any())
            {
                //FormListModel formData = await FormData(conn, transaction, ssmodel.FormID);
                List<FlowApproverModel> list = null;

                if (ssmodel.ValueSetting == StepValueSettingEnum.Value.ToString())
                {
                    //list = await FindDeptSignApprover(conn, transaction, null, ssmodel.Value, nowStep, nowStep.CanFindParentDept);
                }
                else
                {
                    list = await ValueSetting(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                }

                result = await CheckHaveApprover(result, list, nowStep);

            }
            else
            {
                result = await SignCountCheck(conn, transaction, ssmodel, nowStepSignLog);
            }

             
            return result;
        }
        /// <summary>
        /// 條件式
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ssmodel"></param>
        /// <param name="nowStepSignLog"></param>
        /// <param name="nowStep"></param>
        /// <returns></returns>
        public async Task<StepValueResponseModel> Decision(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep)
        {
            StepValueResponseModel result = new StepValueResponseModel();

            bool valueResult = true;

            if (ssmodel.ValueSetting == StepValueSettingEnum.Value.ToString())
            {
                valueResult = Convert.ToBoolean(ssmodel.Value);
            }
            else
            {
                valueResult = await ValueSettingBoolean(conn, transaction, ssmodel, nowStepSignLog, nowStep);
            }


            result.StepAllComplete = true;
            result.Value = valueResult.ToString();


            if (valueResult)
            {
                result.SignResult = SignResultTypeEnum.agree;
            }
            else
            {
                result.SignResult = SignResultTypeEnum.reject;
            }

            return result;
        }
        /// <summary>
        /// 指定部門層級(循序)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ssmodel"></param>
        /// <param name="nowStepSignLog"></param>
        /// <param name="nowStep"></param>
        /// <returns></returns>
        public async Task<StepValueResponseModel> RouteSpecificDeptLevel(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep)
        {
            StepValueResponseModel result = new StepValueResponseModel();

            // 首次進入步驟
            if (nowStepSignLog == null || !nowStepSignLog.Any())
            {
                FormListModel formData = await FormData(conn, transaction, ssmodel.FormID);
                List<FlowApproverModel> list = null;

                if (ssmodel.ValueSetting == StepValueSettingEnum.Value.ToString())
                {
                    if (nowStep.FromWho == "Applicant")
                    {
                        list = await FindDeptSignApprover(conn, transaction, formData.ApplicantID, formData.ApplicantDeptID, nowStep);
                    }
                    else
                    {
                        // 簽核記錄
                        List<SignLogModel> prevStepSignLog = await GetPrevSignLog( conn,  transaction, ssmodel.SSID);


                        // 沒紀錄 從申請人部門開始
                        if (prevStepSignLog == null || !prevStepSignLog.Any())
                        {
                            list = await FindDeptSignApprover(conn, transaction, formData.ApplicantID, formData.ApplicantDeptID, nowStep, true);
                        }
                        else
                        {
                            string deptid = prevStepSignLog.FirstOrDefault().DeptID;
                            list = await FindDeptSignApprover(conn, transaction, null, deptid, nowStep, true);
                        }
                    }
                }
                else
                {
                    list = await ValueSetting(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                }

                result = await CheckHaveApprover(result, list, nowStep);
            }
            else
            {
                result = await SignCountCheck(conn, transaction, ssmodel, nowStepSignLog);
            }

            if (result.StepAllComplete && result.SignResult == SignResultTypeEnum.agree && nowStepSignLog.Any()) 
            {
                // 檢查最後簽核紀錄的部門層級
                string deptid = nowStepSignLog.FirstOrDefault().DeptID;
                int lastDeptLevel = await GetDeptLevel(conn, transaction, deptid);


                if (lastDeptLevel < Convert.ToInt32(ssmodel.Value))
                {
                    // 層級不夠 再上往簽
                    result.List = await FindDeptSignApprover(conn, transaction, null, deptid, nowStep, true);

                    result = await CheckHaveApprover(result, result.List, nowStep);
                }
            }



            return result;
        }
        /// <summary>
        /// 指定部門層級(指定)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="ssmodel"></param>
        /// <param name="nowStepSignLog"></param>
        /// <param name="nowStep"></param>
        /// <returns></returns>
        public async Task<StepValueResponseModel> GotoSpecificDeptLevel(MySqlConnection conn, MySqlTransaction transaction, FlowStepSystemLogModel ssmodel, List<SignLogModel> nowStepSignLog, FlowStepModel nowStep)
        {
            StepValueResponseModel result = new StepValueResponseModel();

            // 首次進入步驟
            if (nowStepSignLog == null || !nowStepSignLog.Any())
            {
                FormListModel formData = await FormData(conn, transaction, ssmodel.FormID);
                List<FlowApproverModel> list = null;

                if (ssmodel.ValueSetting == StepValueSettingEnum.Value.ToString())
                {
                    if (nowStep.FromWho == "Applicant")
                    {
                        list = await FindDeptLevelSignApprover(conn, transaction, formData.ApplicantID, formData.ApplicantDeptID, nowStep);
                    }
                    else
                    {
                        // 簽核記錄
                        List<SignLogModel> prevStepSignLog = await GetPrevSignLog(conn, transaction, ssmodel.SSID);


                        // 沒紀錄 從申請人部門開始
                        if (prevStepSignLog == null || !prevStepSignLog.Any())
                        {
                            list = await FindDeptLevelSignApprover(conn, transaction, formData.ApplicantID, formData.ApplicantDeptID, nowStep);
                        }
                        else
                        {
                            string deptid = prevStepSignLog.FirstOrDefault().DeptID;
                            list = await FindDeptLevelSignApprover(conn, transaction, null, deptid, nowStep);
                        }
                    }
                }
                else
                {
                    list = await ValueSetting(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                }


                var selfList = list.Where(x => x.UID == formData.ApplicantID);

                if (list != null && selfList.Any())
                {
                    if (nowStep.WhenSelfSign == FlowStepWhenTypeEnum.pass.ToString())
                    {
                        result.StepAllComplete = true;
                        result.SignResult = SignResultTypeEnum.agree;
                        result.Value = "WhenSelfSign=Pass";
                        return result;
                    }
                    else if (nowStep.WhenSelfSign == FlowStepWhenTypeEnum.find_parent.ToString())
                    {
                        list = await FindDeptSignApprover(conn, transaction, selfList.FirstOrDefault().UID, selfList.FirstOrDefault().DeptID, nowStep, true);
                    }

                }


                result = await CheckHaveApprover(result, list, nowStep);
            }
            else
            {
                result = await SignCountCheck(conn, transaction, ssmodel, nowStepSignLog);
            }

            return result;
        }
    }
}
