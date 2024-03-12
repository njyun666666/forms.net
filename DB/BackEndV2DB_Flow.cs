using FormsNet.DB.DBClass;
using FormsNet.DB.IDB;
using FormsNet.Enums;
using FormsNet.Models.Flow;
using FormsNet.Models.Form;
using FormsNet.Models.Sign;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormsNet.DB
{
    public class BackEndV2DB_Flow : IBackEndV2DB_Flow
    {
        public readonly string DbName = "BackEndV2DB";
        public string str_conn;
        /// <summary>
        /// 此表單使用流程 步驟清單
        /// </summary>
        List<FlowStepModel> flowList;
        List<FlowStepLineModel> lineList;

        public IDBConnection _dBConnection;
        IBackEndV2DB_Flow_Step db_Step;
        IBackEndV2DB_Sign db_Sign;
        IBackEndV2DB_Form db_Form;


        public BackEndV2DB_Flow(IDBConnection dBConnection, IBackEndV2DB_Flow_Step backEndV2DB_Flow_Step, IBackEndV2DB_Sign backEndV2DB_Sign, IBackEndV2DB_Form backEndV2DB_Form)
        {
            _dBConnection = dBConnection;
            str_conn = _dBConnection.Connection(DbName);
            db_Step = backEndV2DB_Flow_Step;
            db_Sign = backEndV2DB_Sign;
            db_Form = backEndV2DB_Form;
        }

        /// <summary>
        /// 取得表單使用的流程ID
        /// </summary>
        /// <param name="formID"></param>
        /// <returns>FlowID</returns>
        public async Task<string> GetFlowID(string formID)
        {
            string sql = "select FlowID from TB_Form_List where FormID=@in_FormID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", formID, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<string>(str_conn, sql, _params);
        }
        /// <summary>
        /// 設定表單預設流程
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<string> SetDefaultFlowID(string formID)
        {
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", formID, DbType.String);

            string sql = "select DefaultFlow from TB_Form_Setting where FormClass=(select FormClass from TB_Form_List where FormID=@in_FormID limit 1)";

            string FlowID = await AsyncDB.QueryFirstOrDefaultAsync<string>(str_conn, sql, _params);


            sql = "UPDATE `TB_Form_List` SET `FlowID` = @in_FlowID  WHERE `FormID` = @in_FormID; ";

            _params.Add("@in_FlowID", FlowID, DbType.String);

            await AsyncDB.QueryFirstOrDefaultAsync<string>(str_conn, sql, _params);

            return FlowID;
        }
        /// <summary>
        /// 取得表單預設流程
        /// </summary>
        /// <param name="formClass"></param>
        /// <returns></returns>
        public async Task<string> GetDefaultFlowID(string formClass)
        {
            string sql = "select DefaultFlow from TB_Form_Setting where FormClass=@in_formClass ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_formClass", formClass, DbType.String);

            return await AsyncDB.QueryFirstOrDefaultAsync<string>(str_conn, sql, _params);
        }
        /// <summary>
        /// 流程步驟系統記錄
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="model"></param>
        /// <param name="valueModel"></param>
        /// <returns></returns>
        public async Task<int> AddFlowStepSystemLogError(string FormID, string Messagge)
        {
            string sql = "INSERT INTO `TB_Flow_Step_SystemLog`" +
            " (`FormID`,`Message`)" +
            "  VALUES" +
            " (@in_FormID, @in_Messagge) ; ";


            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", FormID, DbType.String);
            _params.Add("@in_Messagge", Messagge, DbType.String);

            return await AsyncDB.ExecuteAsync(str_conn, sql, _params, DoTransaction: false);
        }

        #region flowchart
        /// <summary>
        /// 取得 flowchart用 步驟清單
        /// </summary>
        /// <param name="FlowID"></param>
        /// <returns></returns>
        public async Task<List<FlowchartStepModel>> FlowchartStep(string FlowID)
        {
            string sql = "select StepID, StepType, StepName from TB_Flow_Step where FlowID=@in_FlowID";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FlowID", FlowID, DbType.String);

            return await AsyncDB.QueryAsync<FlowchartStepModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得 flowchart用 路線清單
        /// </summary>
        /// <param name="FlowID"></param>
        /// <returns></returns>
        public async Task<List<FlowchartLineModel>> FlowchartLine(string FlowID)
        {
            string sql = "select `StartStepID`, `EndStepID`, `ValueSetting`, `ResultID`, `ValueFunction`, `FlowchartPath`, `FlowchartDirections` " +
                    " from TB_Flow_Step_Line where StartStepID in ( select StepID from TB_Flow_Step where FlowID=@in_FlowID)";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FlowID", FlowID, DbType.String);

            return await AsyncDB.QueryAsync<FlowchartLineModel>(str_conn, sql, _params);
        }
        #endregion

        #region 表單編號
        /// <summary>
        /// 設定表單編號
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FlowID"></param>
        /// <returns></returns>
        public async Task<string> SetSerial(string FormID,string FlowID)
        {
            string serial = "";

            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 取得表單資料
                        FormListModel formData = await GetFormData(conn, transaction, FormID);
                        
                        if (!string.IsNullOrWhiteSpace(formData.Serial))
                        {
                            return formData.Serial;
                        }


                        // 取得編號規則
                        string sql = "select SerialFormat from TB_Flow_Setting where FlowID=@in_FlowID";
                        
                        DynamicParameters _params = new DynamicParameters();
                        _params.Add("@in_FlowID", FlowID, DbType.String);

                        serial = await conn.QueryFirstOrDefaultAsync<string>(sql, _params, transaction);


                        // 取代時間
                        DateTime ApplicantDate = formData.ApplicantDate.Value;

                        serial = serial.Replace("{YYYY}", ApplicantDate.ToString("yyyy"));
                        serial = serial.Replace("{yyy}", (ApplicantDate.Year-1911).ToString());
                        serial = serial.Replace("{M}", ApplicantDate.Month.ToString());
                        serial = serial.Replace("{MM}", ApplicantDate.ToString("MM"));
                        serial = serial.Replace("{d}", ApplicantDate.Day.ToString());
                        serial = serial.Replace("{dd}", ApplicantDate.ToString("dd"));


                        // 公司別
                        if (serial.Contains("{CompanyTypeID}"))
                        {
                            sql = "select CompanyType from TB_Org_Account_Struct where UID=@in_UID ";
                            _params = new DynamicParameters();
                            _params.Add("@in_UID", formData.ApplicantID, DbType.String);

                            string CompanyTypeID = await conn.QueryFirstOrDefaultAsync<string>(sql, _params, transaction);

                            serial = serial.Replace("{CompanyTypeID}", CompanyTypeID);
                        }


                        // 序號
                        // 先取得最後一筆
                        sql = "select * from TB_Flow_Serial where FormClass=@in_FormClass order by TID desc limit 1 for update";
                        _params = new DynamicParameters();
                        _params.Add("@in_FormClass", formData.FormClass, DbType.String);

                        FlowSerialModel lastSerialModel = await conn.QueryFirstOrDefaultAsync<FlowSerialModel>(sql, _params, transaction);

                        if (lastSerialModel == null)
                        {
                            lastSerialModel = new FlowSerialModel();
                        }


                        // 設定新序號
                        FlowSerialModel newSerialModel = new FlowSerialModel()
                        {
                            FormID = FormID,
                            FormClass = formData.FormClass,
                            Date = ApplicantDate,
                            YID = lastSerialModel.Date.Year == ApplicantDate.Year ? lastSerialModel.YID + 1 : 1,
                            MID = lastSerialModel.Date.Year == ApplicantDate.Year && lastSerialModel.Date.Month == ApplicantDate.Month ? lastSerialModel.MID + 1 : 1,
                            DID = lastSerialModel.Date.Date == ApplicantDate.Date ? lastSerialModel.DID + 1 : 1,
                            TID = lastSerialModel.TID + 1
                        };

                        // insert 
                        sql = "INSERT INTO `TB_Flow_Serial`" +
                            " (`FormID`,`FormClass`,`Date`,`YID`,`MID`,`DID`,`TID`)" +
                            " VALUES " +
                            " (@in_FormID, @in_FormClass, @in_Date, @in_YID, @in_MID, @in_DID, @in_TID);";


                        _params = new DynamicParameters();
                        _params.Add("@in_FormID", newSerialModel.FormID, DbType.String);
                        _params.Add("@in_FormClass", newSerialModel.FormClass, DbType.String);
                        _params.Add("@in_Date", newSerialModel.Date, DbType.DateTime);
                        _params.Add("@in_YID", newSerialModel.YID, DbType.Int32);
                        _params.Add("@in_MID", newSerialModel.MID, DbType.Int32);
                        _params.Add("@in_DID", newSerialModel.DID, DbType.Int32);
                        _params.Add("@in_TID", newSerialModel.TID, DbType.Int32);

                        await conn.ExecuteAsync(sql, _params, transaction);


                        // 序號處理
                        serial = SerialIDProcess("yid", serial, newSerialModel.YID.ToString());
                        serial = SerialIDProcess("mid", serial, newSerialModel.MID.ToString());
                        serial = SerialIDProcess("did", serial, newSerialModel.DID.ToString());
                        serial = SerialIDProcess("tid", serial, newSerialModel.TID.ToString());


                        // 回寫表單Serial欄位
                        sql = "UPDATE `TB_Form_List` SET `Serial` = @in_Serial  WHERE `FormID` = @in_FormID; ";
                        _params = new DynamicParameters();
                        _params.Add("@in_FormID", FormID, DbType.String);
                        _params.Add("@in_Serial", serial, DbType.String);

                        await conn.ExecuteAsync(sql, _params, transaction);


                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }

            }

            return serial;
        }
        public string SerialIDProcess(string formatName, string serial, string id)
        {
            string yid_pattern = "\\{"+ formatName + "\\-[\\d]+\\}";
            Match yid_match = Regex.Match(serial, yid_pattern, RegexOptions.IgnoreCase);
            if (yid_match.Success)
            {
                string yid_format = yid_match.Value;
                int num = StringGetNumber(yid_format);

                string yid = id.PadLeft(num, '0');
                serial = serial.Replace(yid_format, yid);
            }
            return serial;
        }
        public int StringGetNumber(string input)
        {
            // 取得數字
            string num_pattern = "[\\d]+";
            Match num_match = Regex.Match(input, num_pattern, RegexOptions.IgnoreCase);

            int number = 0;

            if (num_match.Success)
            {
                number = Convert.ToInt32(num_match.Value);
            }
            return number;
        }
        #endregion
        /// <summary>
        /// 取得表單資料
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="FormID"></param>
        /// <returns></returns>
        public async Task<FormListModel> GetFormData(MySqlConnection conn, MySqlTransaction transaction, string FormID)
        {
            string sql = "select * from TB_Form_List where FormID=@in_FormID";
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", FormID, DbType.String);
            return await conn.QueryFirstOrDefaultAsync<FormListModel>(sql, _params, transaction);
        }
        /// <summary>
        /// 取得流程
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="FlowID"></param>
        public async Task<List<FlowStepModel>> GetFlowList(MySqlConnection conn, MySqlTransaction transaction, string FlowID)
        {
            flowList = await GetFlowStepByFlowID(conn, transaction, FlowID);
            lineList= await GetFlowStepLine(conn, transaction, FlowID);
            return flowList;
        }
        /// <summary>
        /// 取得流程路線
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="FlowID"></param>
        /// <returns></returns>
        public async Task<List<FlowStepLineModel>> GetFlowStepLine(MySqlConnection conn, MySqlTransaction transaction, string FlowID)
        {
            string sql = "select * from TB_Flow_Step_Line where StartStepID in (select StepID from TB_Flow_Step where FlowID=@in_FlowID)";
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FlowID", FlowID, DbType.String);

            return (await conn.QueryAsync<FlowStepLineModel>(sql, _params, transaction)).ToList();
        }
        public async Task<SignFlowDataModel> GetSignFlowData(MySqlConnection conn, MySqlTransaction transaction, Int64 SignID)
        {
            string sql = "select s.SSID, f.FlowID from TB_Sign_Log s join TB_Form_List f on s.FormID=f.FormID where s.ID=@in_ID limit 1 for update";
            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_ID", SignID, DbType.Int64);

            return await conn.QueryFirstOrDefaultAsync<SignFlowDataModel>(sql, _params, transaction);
        }

        /// <summary>
        /// 開始流程
        /// </summary>
        /// <param name="FormID"></param>
        /// <param name="FlowID"></param>
        /// <returns></returns>
        public async Task<int> Start(string FormID, string FlowID)
        {
            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 取得流程
                        await GetFlowList(conn, transaction, FlowID);

                        // 取得開始步驟
                        FlowStepModel startStep = flowList.Where(x => x.StepType == StepTypeEnum.Start).FirstOrDefault();
                        StepValueResponseModel valueModel = new StepValueResponseModel() { Value = "true" };

                        Int64 ssid = await AddFlowStepSystemLog(conn, transaction, FormID, startStep, valueModel, 0);

                        var a = await FindNextApprover(conn, transaction, FormID, ssid);
                        //await FindNextStep(conn, transaction, ssid, SignResultTypeEnum.agree);


                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }

            }

            return 1;
        }
        /// <summary>
        /// 簽核後
        /// </summary>
        /// <param name="signModel"></param>
        /// <returns></returns>
        public async Task<int> Sign(SignModel signModel)
        {
            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 取得FlowID, SSID
                        SignFlowDataModel signFlowDataModel = await GetSignFlowData(conn, transaction, signModel.ID);
                        Int64 SSID = signFlowDataModel.SSID;
                        string FlowID = signFlowDataModel.FlowID;


                        // 取得流程
                        await GetFlowList(conn, transaction, FlowID);


                        var a = await FindNextApprover(conn, transaction, signModel.FormID, SSID);


                        //await FindNextStep(conn, transaction, ssid, SignResultTypeEnum.agree);


                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }

            }

            return 1;
        }
        /// <summary>
        /// 修改表單狀態
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="SignFormModel"></param>
        /// <returns></returns>
        public async Task<int> SignSetResult(string uid, SignFormModel signModel)
        {
            using (MySqlConnection conn = new MySqlConnection(str_conn))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = "";
                        DynamicParameters _params = new DynamicParameters();
                        List<FlowApproverModel> list = null;


                        // 檢查表單還沒結束
                        sql = "select exists (" +
                              "  select 1 from TB_Form_List where FormID=@in_FormID and ResultID is null for update" +
                              " ) ";


                        _params = new DynamicParameters();
                        _params.Add("@in_FormID", signModel.FormID, DbType.String);

                        bool auth = await conn.QueryFirstOrDefaultAsync<bool>(sql, _params, transaction);

                        if (!auth)
                        {
                            await transaction.RollbackAsync();
                            return 0;
                        }



                        // 取得修改人uid deptid
                        switch (signModel.SignOptionID)
                        {
                            // 申請人修改選項 撤回
                            case SignOptionEnum.applicant_edit_cancel:


                                // 取得申請人 UID DeptID
                                sql = "select ApplicantID as UID, ApplicantDeptID as DeptID from TB_Form_List where FormID=@in_FormID ";
                                _params = new DynamicParameters();
                                _params.Add("@in_FormID", signModel.FormID, DbType.String);

                                list = (await conn.QueryAsync<FlowApproverModel>(sql, _params, transaction)).ToList();


                                break;


                            default:

                                // 取得登入者 UID DeptID
                                sql = "select UID, DeptID from TB_Org_Account_Struct where UID=@in_UID and Status=1 order by Main desc limit 1 ";
                                _params = new DynamicParameters();
                                _params.Add("@in_UID", uid, DbType.String);

                                list = (await conn.QueryAsync<FlowApproverModel>(sql, _params, transaction)).ToList();

                                break;
                        }




                        // 新增簽核記錄
                        await db_Sign.AddSignLog(conn, transaction, signModel.FormID, null, list, null, signModel, 1);


                        // 更新表單簽核結果
                        await db_Form.SetResuult(conn, transaction, signModel.FormID, signModel.SignResultID);

                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }

            }

            return 1;
        }
        /// <summary>
        /// 找下一步驟簽核人
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="FormID"></param>
        /// <param name="NowSSID"></param>
        /// <returns></returns>
        public async Task<bool> FindNextApprover(MySqlConnection conn, MySqlTransaction transaction, string FormID, Int64 NowSSID)
        {
            // 取得步驟設定
            FlowStepSystemLogModel ssmodel = await GetFlowStepSystemLog(conn, transaction, NowSSID);
            FlowStepModel nowStep = flowList.Where(x => x.StepID == ssmodel.StepID).FirstOrDefault();

            // 此步驟全部簽核記錄
            List<SignLogModel> nowStepSignLog = await db_Sign.GetSignLogList(conn, transaction, NowSSID);


            StepValueResponseModel nowApproverModel = new StepValueResponseModel();



            // 駁回結束
            if (nowStepSignLog.Where(x => x.Status == 1 && x.SignOptionID == (int)SignOptionEnum.sign_other_reject_end).Any())
            {
                await db_Form.SetResuult(conn, transaction, FormID, SignResultTypeEnum.reject);
                return true;
            }
            // 撤回
            else if (nowStepSignLog.Where(x => x.Status == 1 && x.SignOptionID == (int)SignOptionEnum.applicant_edit_cancel).Any())
            {
                await db_Form.SetResuult(conn, transaction, FormID, SignResultTypeEnum.cancel);
                return true;
            }


            switch (nowStep.StepType)
            {
                case StepTypeEnum.Start:
                    // 步驟=開始 直接找下一步驟
                    nowApproverModel.StepAllComplete = true;
                    nowApproverModel.SignResult = SignResultTypeEnum.agree;
                    break;

                case StepTypeEnum.ApplicantSelf:
                    nowApproverModel = await db_Step.ApplicantSelf(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                    break;

                case StepTypeEnum.ApplicantDepartment:
                    nowApproverModel = await db_Step.ApplicantDepartment(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                    break;

                case StepTypeEnum.DesignateList:
                    nowApproverModel = await db_Step.DesignateList(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                    break;
                    

                case StepTypeEnum.GotoSpecificDepartment:
                    nowApproverModel = await db_Step.GotoSpecificDepartment(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                    break;

                case StepTypeEnum.GotoSpecificJobTitle:
                    //nowApproverModel = await db_Step.GotoSpecificDepartment(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                    break;

                case StepTypeEnum.Decision:
                    nowApproverModel = await db_Step.Decision(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                    break;

                case StepTypeEnum.RouteSpecificDeptLevel:
                    nowApproverModel = await db_Step.RouteSpecificDeptLevel(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                    break;

                case StepTypeEnum.GotoSpecificDeptLevel:
                    nowApproverModel = await db_Step.GotoSpecificDeptLevel(conn, transaction, ssmodel, nowStepSignLog, nowStep);
                    break;
                    



                // 結束流程 表單回寫結果
                case StepTypeEnum.EndAgree:
                    await db_Form.SetResuult(conn, transaction, FormID, SignResultTypeEnum.agree);
                    return true;
                    break;

                case StepTypeEnum.EndReject:
                    await db_Form.SetResuult(conn, transaction, FormID, SignResultTypeEnum.reject);
                    return true;
                    break;

                case StepTypeEnum.EndNotEstablished:
                    await db_Form.SetResuult(conn, transaction, FormID, SignResultTypeEnum.not_established);
                    return true;
                    break;

                case StepTypeEnum.EndInvalidate:
                    await db_Form.SetResuult(conn, transaction, FormID, SignResultTypeEnum.invalidate);
                    return true;
                    break;

                case StepTypeEnum.EndCaseCompleted:
                    await db_Form.SetResuult(conn, transaction, FormID, SignResultTypeEnum.case_completed);
                    return true;
                    break;

                default:
                    break;
            }



            // 此步驟 新簽核人
            if (nowApproverModel.List.Any())
            {
                Int64 nextSSID = await AddFlowStepSystemLog(conn, transaction, FormID, nowStep, nowApproverModel, 1);
                await UpdateFlowStepSystemLog_NextSSID(conn, transaction, NowSSID, nextSSID);
                await db_Sign.AddSignLog(conn, transaction, FormID, nextSSID, nowApproverModel.List, nowStep);
            }
            // 此步驟全部完成 找下一步驟
            else if (nowApproverModel.StepAllComplete && nowApproverModel.SignResult != SignResultTypeEnum.sign_await)
            {
                await FindNextStep(conn, transaction, FormID, NowSSID, nowStep, nowApproverModel.SignResult);
            }

            return true;
        }

        public async Task<Int64> FindNextStep(MySqlConnection conn, MySqlTransaction transaction, string FormID, Int64 NowSSID, FlowStepModel nowSetp, SignResultTypeEnum stepSignResult)
        {
            // 取得下一步驟
            FlowStepModel nextStepModel = null;
            bool reject_end = false;

            var nextLineList = lineList.Where(x => x.StartStepID == nowSetp.StepID).ToList();



            if (stepSignResult == SignResultTypeEnum.agree)
            {
                var agreeLine = lineList.Where(x => x.StartStepID == nowSetp.StepID && x.ValueSetting == StepValueSettingEnum.ResultID.ToString() && x.ResultID == SignResultTypeEnum.agree).FirstOrDefault();
                nextStepModel = flowList.Where(x => x.StepID == agreeLine.EndStepID).FirstOrDefault();
            }
            else if (stepSignResult == SignResultTypeEnum.reject)
            {
                var rejectLine = lineList.Where(x => x.StartStepID == nowSetp.StepID && x.ResultID == SignResultTypeEnum.reject).FirstOrDefault();

                if (rejectLine != null)
                {
                    nextStepModel = flowList.Where(x => x.StepID == rejectLine.EndStepID).FirstOrDefault();
                }

                // 沒設定駁回路線，原路找上一步簽核步驟
                if (nextStepModel == null)
                {
                    FlowStepSystemLogModel prevSSLog = await GetPrevFlowStepSystemNeedSign(conn, transaction, NowSSID);

                    if (prevSSLog != null)
                    {
                        nextStepModel = flowList.Where(x => x.StepID == prevSSLog.StepID).FirstOrDefault();
                    }
                }


                // 沒有下一步/下一步是[開始]的話，就駁回結束
                if (nextStepModel == null || nextStepModel.StepType == StepTypeEnum.Start)
                {
                    reject_end = true;
                }

            }


            StepValueResponseModel valueModel = new StepValueResponseModel();




            if (reject_end)
            {
                await db_Form.SetResuult(conn, transaction, FormID, SignResultTypeEnum.reject);
            }
            else
            {
                Int64 nextSSID = await AddFlowStepSystemLog(conn, transaction, FormID, nextStepModel, valueModel, 0);
                await UpdateFlowStepSystemLog_NextSSID(conn, transaction, NowSSID, nextSSID);
                await FindNextApprover(conn, transaction, FormID, nextSSID);
            }


            return 0;
        }


        public async Task<List<FlowStepModel>> GetFlowStepByFlowID(MySqlConnection conn, MySqlTransaction transaction, string FlowID)
        {
            string sql = "select * from TB_Flow_Step where FlowID = @in_flowID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FlowID", FlowID, DbType.String);

            return (await conn.QueryAsync<FlowStepModel>(sql, _params, transaction)).ToList();
        }
        public async Task<FlowStepModel> GetFlowStepByStepType(MySqlConnection conn, MySqlTransaction transaction, string FlowID, StepTypeEnum StepType)
        {
            string sql = "select * from TB_Flow_Step where FlowID = @in_flowID and StepType=@in_StepType ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FlowID", FlowID, DbType.String);
            _params.Add("@in_StepType", StepType, DbType.Int32);

            return await conn.QueryFirstOrDefaultAsync<FlowStepModel>(sql, _params, transaction);
        }
        public async Task<FlowStepModel> GetFlowStepByStepID(int StepID)
        {
            string sql = "select * from TB_Flow_Step where StepID=@in_StepID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_StepID", StepID, DbType.Int32);

            return await AsyncDB.QueryFirstOrDefaultAsync<FlowStepModel>(str_conn, sql, _params);
        }
        public async Task<FlowStepModel> GetFlowStepByStepID(MySqlConnection conn, MySqlTransaction transaction, int StepID)
        {
            string sql = "select * from TB_Flow_Step where StepID=@in_StepID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_StepID", StepID, DbType.Int32);

            return await conn.QueryFirstOrDefaultAsync<FlowStepModel>(sql, _params, transaction);
        }
        /// <summary>
        /// 流程步驟系統記錄
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="model"></param>
        /// <param name="valueModel"></param>
        /// <returns></returns>
        public async Task<Int64> AddFlowStepSystemLog(MySqlConnection conn, MySqlTransaction transaction, string FormID, FlowStepModel model, StepValueResponseModel valueModel, Int16 NeedSign)
        {
            string sql = "INSERT INTO `TB_Flow_Step_SystemLog`" +
            " (`FormID`,`StepID`,`FlowID`,`ValueSetting`,`Value`,`ValueField`,`ValueFunction`,`ValueResponse`,`SignCountType`,`AgreeRatio`,`NextSSID`,`NeedSign`)" +
            "  VALUES" +
            " (@in_FormID, @in_StepID, @in_FlowID, @in_ValueSetting, @in_Value, @in_ValueField, @in_ValueFunction, @in_ValueResponse, @in_SignCountType, @in_AgreeRatio, @in_NextSSID, @in_NeedSign) ; ";

            sql += " select LAST_INSERT_ID();";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FormID", FormID, DbType.String);
            _params.Add("@in_StepID", model.StepID, DbType.Int32);
            _params.Add("@in_FlowID", model.FlowID, DbType.String);
            _params.Add("@in_ValueSetting", model.ValueSetting, DbType.String);
            _params.Add("@in_Value", model.Value, DbType.String);
            _params.Add("@in_ValueField", model.ValueField, DbType.String);
            _params.Add("@in_ValueFunction", model.ValueFunction, DbType.String);
            _params.Add("@in_ValueResponse", JsonSerializer.Serialize(valueModel), DbType.String);
            _params.Add("@in_SignCountType", model.SignCountType, DbType.String);
            _params.Add("@in_AgreeRatio", model.AgreeRatio, DbType.String);
            _params.Add("@in_NextSSID", null, DbType.Int64);
            _params.Add("@in_NeedSign", NeedSign, DbType.Int32);

            return await conn.ExecuteScalarAsync<Int64>(sql, _params, transaction);
        }
        public async Task<int> UpdateFlowStepSystemLog_ValueResponse(MySqlConnection conn, MySqlTransaction transaction, Int64 SSID, StepValueResponseModel valueModel)
        {
            string sql = "UPDATE `TB_Flow_Step_SystemLog` SET" +
                            " `ValueResponse` = @in_ValueResponse" +
                        "  WHERE `SSID` = @in_SSID; ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_SSID", SSID, DbType.Int64);
            _params.Add("@in_ValueResponse", JsonSerializer.Serialize(valueModel), DbType.String);

            return await conn.ExecuteAsync(sql, _params, transaction);
        }
        public async Task<int> UpdateFlowStepSystemLog_NextSSID(MySqlConnection conn, MySqlTransaction transaction, Int64 SSID, Int64 NextSSID)
        {
            string sql = "UPDATE `TB_Flow_Step_SystemLog` SET" +
                            " `NextSSID` = @in_NextSSID" +
                        "  WHERE `SSID` = @in_SSID; ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_SSID", SSID, DbType.Int64);
            _params.Add("@in_NextSSID", NextSSID, DbType.Int64);

            return await conn.ExecuteAsync(sql, _params, transaction);
        }
        public async Task<FlowStepSystemLogModel> GetFlowStepSystemLog(MySqlConnection conn, MySqlTransaction transaction, Int64 NowSSID)
        {
            string sql = "select * from TB_Flow_Step_SystemLog where SSID=@in_SSID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_SSID", NowSSID, DbType.Int64);

            return await conn.QueryFirstOrDefaultAsync<FlowStepSystemLogModel>(sql, _params, transaction);
        }
        /// <summary>
        /// 取得前一步驟
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="NowSSID"></param>
        /// <returns></returns>
        public async Task<FlowStepSystemLogModel> GetPrevFlowStepSystemLog(MySqlConnection conn, MySqlTransaction transaction, Int64 NextSSID)
        {
            string sql = "select * from TB_Flow_Step_SystemLog where NextSSID=@in_NextSSID ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_NextSSID", NextSSID, DbType.Int64);

            return await conn.QueryFirstOrDefaultAsync<FlowStepSystemLogModel>(sql, _params, transaction);
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


    }
}
