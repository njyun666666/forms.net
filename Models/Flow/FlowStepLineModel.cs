using FormsNet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Flow
{
    public class FlowStepLineModel
    {
        /// <summary>
        /// 路線代號
        /// </summary>
        public int LineID { get; set; }
        /// <summary>
        /// 開始步驟
        /// </summary>
        public int StartStepID { get; set; }
        /// <summary>
        /// 結束步驟
        /// </summary>
        public int EndStepID { get; set; }
        /// <summary>
        /// 路線設定
        /// </summary>
        public string ValueSetting { get; set; }
        /// <summary>
        /// 依照步驟簽核結果
        /// </summary>
        public SignResultTypeEnum ResultID { get; set; }
        /// <summary>
        /// 指定函式
        /// </summary>
        public string ValueFunction { get; set; }
    }
}
