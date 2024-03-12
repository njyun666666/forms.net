using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FormsNet.Models.Form
{
    public class FormListModel
    {
        /// <summary>
        /// 表單代號
        /// </summary>
        [JsonPropertyName("formID")]
        public string FormID { get; set; }
        /// <summary>
        /// 表單類型
        /// </summary>
        [JsonPropertyName("formClass")]
        public string FormClass { get; set; }
        /// <summary>
        /// 表單編號
        /// </summary>
        [JsonPropertyName("serial")]
        public string Serial { get; set; }
        /// <summary>
        /// 填單人
        /// </summary>
        [JsonPropertyName("fillerID")]
        public string FillerID { get; set; }
        /// <summary>
        /// 填單人姓名
        /// </summary>
        [JsonPropertyName("fillerName")]
        public string FillerName { get; set; }
        /// <summary>
        /// 填單人部門代號
        /// </summary>
        [JsonPropertyName("fillerDeptID")]
        public string FillerDeptID { get; set; }
        /// <summary>
        /// 填單人部門
        /// </summary>
        [JsonPropertyName("fillerDept")]
        public string FillerDept { get; set; }
        /// <summary>
        /// 申請人
        /// </summary>
        [JsonPropertyName("applicantID")]
        public string ApplicantID { get; set; }
        /// <summary>
        /// 申請人姓名
        /// </summary>
        [JsonPropertyName("applicantName")]
        public string ApplicantName { get; set; }
        /// <summary>
        /// 申請人部門代號
        /// </summary>
        [JsonPropertyName("applicantDeptID")]
        public string ApplicantDeptID { get; set; }
        /// <summary>
        /// 申請人部門
        /// </summary>
        [JsonPropertyName("applicantDept")]
        public string ApplicantDept { get; set; }
        /// <summary>
        /// 申請時間
        /// </summary>
        [JsonPropertyName("applicantDate")]
        public DateTime? ApplicantDate { get; set; }
        /// <summary>
        /// 級別
        /// </summary>
        [JsonPropertyName("levelID")]
        public int? LevelID { get; set; }
        /// <summary>
        /// 狀態 0=草稿, 1=正式, -1=刪除
        /// </summary>
        [JsonPropertyName("status")]
        public Int16? Status { get; set; }
        /// <summary>
        /// 修改時間
        /// </summary>
        [JsonPropertyName("updateDate")]
        public DateTime? UpdateDate { get; set; }
        /// <summary>
        /// 簽核流程代號
        /// </summary>
        [JsonPropertyName("flowID")]
        public string FlowID { get; set; }
        /// <summary>
        /// 簽核結果代號
        /// </summary>
        [JsonPropertyName("resultID")]
        public int? ResultID { get; set; }
        /// <summary>
        /// 檔案群組代號
        /// </summary>
        [JsonPropertyName("fileGroupID")]
        public string FileGroupID { get; set; }
        
    }
}