using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Enums
{
    public enum SignOptionEnum
    {
        /// <summary>
        /// 申請表單選項 同意
        /// </summary>
        applicant_agree = 1,
        /// <summary>
        /// 申請表單選項 駁回
        /// </summary>
        applicant_reject = 2,
        /// <summary>
        /// 申請表單選項 草稿
        /// </summary>
        applicant_draft = 3,
        /// <summary>
        /// 簽核選項 同意
        /// </summary>
        sign_agree = 4,
        /// <summary>
        /// 簽核選項 駁回
        /// </summary>
        sign_reject = 5,
        /// <summary>
        /// 簽核選項 草稿
        /// </summary>
        sign_draft = 6,
        /// <summary>
        /// 申請人修改選項 撤回
        /// </summary>
        applicant_edit_cancel = 7,
        /// <summary>
        /// 簽核選項其他 駁回結束
        /// </summary>
        sign_other_reject_end = 8,
        /// <summary>
        /// 表單權限修改狀態選項 未成立
        /// </summary>
        auth_not_established = 9,
        /// <summary>
        /// 表單權限修改狀態選項 作廢
        /// </summary>
        auth_invalidate = 10
    }
}
