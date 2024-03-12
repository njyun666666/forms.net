using FormsNet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Sign
{
    public class SignOptionModel
    {
        /// <summary>
        /// 代號
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 選項名稱
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 簽核結果
        /// </summary>
        public int ResultID { get; set; }
        /// <summary>
        /// 類型
        /// </summary>
        public int TypeID { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public Int16 Status { get; set; }
        /// <summary>
        /// 指定步驟增加選項
        /// </summary>
        public int StepID { get; set; }
        /// <summary>
        /// 說明
        /// </summary>
        public string Description { get; set; }
    }

    public class GetSignOptionModel
    {
        public SignOptionTypeEnum TypeID { get; set; }
        public int? StepID { get; set; }
        public string FormID { get; set; }
    }


}
