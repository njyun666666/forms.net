using FormsNet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormsNet.Models.Sign
{
    public class SignFormModel
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public Int64 ID { get; set; }
        /// <summary>
        /// 表單代號
        /// </summary>
        public string FormID { get; set; }
        /// <summary>
        /// 步驟代號
        /// </summary>
        public int StepID { get; set; }
        /// <summary>
        /// 步驟名稱
        /// </summary>
        public string StepName { get; set; }
        /// <summary>
        /// 簽核選項代號
        /// </summary>
        public SignOptionEnum SignOptionID { get; set; }
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
        /// 狀態
        /// </summary>
        public Int16 Status { get; set; }
        /// <summary>
        /// 表單送達時間
        /// </summary>
        public DateTime ArrivalDate { get; set; }
        /// <summary>
        /// 檔案群組代號
        /// </summary>
        public string FileGroupID { get; set; }
        /// <summary>
        /// 步驟說明
        /// </summary>
        public string StepDescription { get; set; }
    }
    public class SignModel: SignFormModel
    {
        public JsonElement FormData{ get; set; }
    }
}
