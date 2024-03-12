using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Flow
{
    public class FlowStepSystemLogModel
    {
        public Int64 SSID { get; set; }
        public string FormID { get; set; }
        /// <summary>
        /// 步驟代號
        /// </summary>
        public int StepID { get; set; }
        /// <summary>
        /// 流程代號
        /// </summary>
        public string FlowID { get; set; }
        /// <summary>
        /// 取得目標結果/簽核人
        /// </summary>
        public string ValueSetting { get; set; }
        /// <summary>
        /// 目標指定值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 目標指定欄位
        /// </summary>
        public string ValueField { get; set; }
        /// <summary>
        /// 目標指定函式
        /// </summary>
        public string ValueFunction { get; set; }
        /// <summary>
        /// 簽核人數設定
        /// </summary>
        public int SignCountType { get; set; }
        /// <summary>
        /// 同意比例%
        /// </summary>
        public int AgreeRatio { get; set; }
        public Int64 NextSSID { get; set; }
        public Int16 NeedSign { get; set; }
    }
}
