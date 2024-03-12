using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Enums
{
    public enum SignOptionTypeEnum
    {
        /// <summary>
        /// 申請人填單選項
        /// </summary>
        applicant = 1,
        /// <summary>
        /// 標準簽核選項
        /// </summary>
        sign = 2,
        /// <summary>
        /// 其它簽核選項
        /// </summary>
        sign_other = 3,
        /// <summary>
        /// 申請人修改選項
        /// </summary>
        applicant_edit = 4
    }
}
