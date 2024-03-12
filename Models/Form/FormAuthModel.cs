using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Form
{
    public class FormAuthModel
    {
        /// <summary>
        /// 使用者代號
        /// </summary>
        public string UID { get; set; }
        /// <summary>
        /// 表單
        /// </summary>
        public string FormClass { get; set; }
        /// <summary>
        /// 權限類型代號
        /// </summary>
        public int AuthType { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public int Status { get; set; }
    }
    public class FormAuthTypeModel
    {
        /// <summary>
        /// 權限類型代號
        /// </summary>
        public int AuthType { get; set; }
        /// <summary>
        /// 權限類型名稱
        /// </summary>
        public string TypeName { get; set; }
    }
    public class FormAuthStatusModel : FormAuthTypeModel
    {
        public int Status { get; set; }
    }

    public class FormAuthSettingReqeustModel
    {
        public string UID { get; set; }
    }

    public class FormAuthViewModel
    {
        public string TypeName { get; set; }
        public List<FormSettingAuthViewModel> children { get; set; }
    }

    public class FormSettingAuthViewModel
    {
        public string FormClass { get; set; }
        public string FormName { get; set; }
        public List<FormAuthStatusModel> Auth { get; set; }
    }

    public class FormAuthEditModel
    {
        public string UID { get; set; }
        public string FormClass { get; set; }
        public int AuthType { get; set; }
        public Int16 Status { get; set; }
    }

}
