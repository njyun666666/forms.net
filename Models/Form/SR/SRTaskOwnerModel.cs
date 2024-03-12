using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FormsNet.Models.Form.SR
{
    public class SRTaskOwnerModel
    {
        /// <summary>
        /// 代號
        /// </summary>
        [JsonPropertyName("id")]
        public int? ID { get; set; }
        /// <summary>
        /// 表單代號
        /// </summary>
        [JsonIgnore]
        [JsonPropertyName("formID")]
        public string FormID { get; set; }
        /// <summary>
        /// 承辦人代號
        /// </summary>
        [JsonPropertyName("uid")]
        public string UID { get; set; }
        /// <summary>
        /// 承辦人
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// 技術工作天數
        /// </summary>
        [JsonPropertyName("workDay")]
        public int? WorkDay { get; set; }
        /// <summary>
        /// 測試天數
        /// </summary>
        [JsonPropertyName("testDay")]
        public int? TestDay { get; set; }
        /// <summary>
        /// 預計開工日期
        /// </summary>
        [JsonPropertyName("estStartDate")]
        public DateTime? EstStartDate { get; set; }
        /// <summary>
        /// 預計完成日期
        /// </summary>
        [JsonPropertyName("estEndDate")]
        public DateTime? EstEndDate { get; set; }
        /// <summary>
        /// 技術實際開工日期
        /// </summary>
        [JsonPropertyName("realStartDate")]
        public DateTime? RealStartDate { get; set; }

    }
}
