using FormsNet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Flow
{
    public class FlowStepModel
    {
        /// <summary>
        /// 步驟代號
        /// </summary>
        public int StepID { get; set; }
        /// <summary>
        /// 流程代號
        /// </summary>
        public string FlowID { get; set; }
        /// <summary>
        /// 步驟類型代號
        /// </summary>
        public StepTypeEnum StepType { get; set; }
        /// <summary>
        /// 從誰開始找簽核人
        /// </summary>
        public string FromWho { get; set; }
        /// <summary>
        /// 步驟名稱
        /// </summary>
        public string StepName { get; set; }
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
        public string ValueFieldBoolean { get; set; }
        public string ValueFunctionBoolean { get; set; }
        /// <summary>
        /// 簽核人數設定
        /// </summary>
        public int SignCountType { get; set; }
        /// <summary>
        /// 同意比例%
        /// </summary>
        public int AgreeRatio { get; set; }
        /// <summary>
        /// 步驟說明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 找不到簽核人，往上找
        /// </summary>
        public Int16 CanFindParentDept { get; set; }
        /// <summary>
        /// 找不到簽核人時?
        /// </summary>
        public string WhenNotFoundApprover { get; set; }
        /// <summary>
        /// 是申請人自己簽核時？
        /// </summary>
        public string WhenSelfSign { get; set; }
    }
}
