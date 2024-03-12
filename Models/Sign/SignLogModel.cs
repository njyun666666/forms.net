using FormsNet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Sign
{
    public class SignLogModel
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public Int64 ID { get; set; }
        /// <summary>
        /// 步驟系統代號
        /// </summary>
        public Int64 SSID { get; set; }
        /// <summary>
        /// 表單代號
        /// </summary>
        public string FormID { get; set; }
        /// <summary>
        /// 步驟代號
        /// </summary>
        public Int64 StepID { get; set; }
        /// <summary>
        /// 步驟名稱
        /// </summary>
        public string StepName { get; set; }
        /// <summary>
        /// 簽核人代號
        /// </summary>
        public string ApproverID { get; set; }
        /// <summary>
        /// 簽核人
        /// </summary>
        public string Approver { get; set; }
        /// <summary>
        /// 簽核人部門代號
        /// </summary>
        public string DeptID { get; set; }
        /// <summary>
        /// 簽核人部門
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 簽核人職稱代號
        /// </summary>
        public int TitleID { get; set; }
        /// <summary>
        /// 簽核人職稱
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 簽核選項代號
        /// </summary>
        public int SignOptionID { get; set; }
        /// <summary>
        /// 簽核選項文字
        /// </summary>
        public string SignOption { get; set; }
        /// <summary>
        /// 簽核結果代號
        /// </summary>
        public SignResultTypeEnum SignResultID { get; set; }
        /// <summary>
        /// 簽核意見
        /// </summary>
        public string SignContent { get; set; }
        /// <summary>
        /// 簽核時間
        /// </summary>
        public DateTime SignDate { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public Int16 Status { get; set; }
        /// <summary>
        /// 表單送達時間
        /// </summary>
        public DateTime ArrivalDate { get; set; }
        /// <summary>
        /// 修改時間
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 檔案群組代號
        /// </summary>
        public string FileGroupID { get; set; }
    }

    public class SignLogViewModel
    {
        /// <summary>
        /// <summary>
        /// 步驟名稱
        /// </summary>
        public string StepName { get; set; }
        /// <summary>
        /// 簽核人
        /// </summary>
        public string Approver { get; set; }
        /// <summary>
        /// 簽核人部門
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 簽核選項文字
        /// </summary>
        public string SignOption { get; set; }
        /// <summary>
        /// 簽核結果代號
        /// </summary>
        public int SignResultID { get; set; }
        /// <summary>
        /// 簽核意見
        /// </summary>
        public string SignContent { get; set; }
        /// <summary>
        /// 簽核時間
        /// </summary>
        public DateTime SignDate { get; set; }
    }

    public class SignLogRequestModel
    {
        public string FormID { get; set; }
    }


}
