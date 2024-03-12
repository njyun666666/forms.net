using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Enums
{
    /// <summary>
    /// 簽核結果類型 TB_Sign_ResultType
    /// </summary>
    public enum SignResultTypeEnum
    {
        /// <summary>
        /// 草稿
        /// </summary>
        draft = 0,
        /// <summary>
        /// 同意
        /// </summary>
        agree = 1,
        /// <summary>
        /// 駁回
        /// </summary>
        reject = 2,
        /// <summary>
        /// 撤單
        /// </summary>
        cancel = 3,
        /// <summary>
        /// 等待簽核
        /// </summary>
        sign_await = 4,
        /// <summary>
        /// 未成立
        /// </summary>
        not_established = 5,
        /// <summary>
        /// 作廢
        /// </summary>
        invalidate = 6,
        /// <summary>
        /// 結案
        /// </summary>
        case_completed = 7
    }
}
