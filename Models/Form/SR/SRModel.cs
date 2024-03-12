using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FormsNet.Models.Form.SR
{
    public class SRModel : FormListModel
    {
        /// <summary>
        /// 專案屬性
        /// </summary>
        [JsonPropertyName("type")]
        public int? Type { get; set; }
        /// <summary>
        /// 專案屬性 其他
        /// </summary>
        [JsonPropertyName("other")]
        public string Other { get; set; }
        /// <summary>
        /// 主旨
        /// </summary>
        [JsonPropertyName("subject")]
        public string Subject { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }
        /// <summary>
        /// 正式上線日期
        /// </summary>
        [JsonPropertyName("onlineDate")]
        public DateTime? OnlineDate { get; set; }
        /// <summary>
        /// 有手機資料
        /// </summary>
        [JsonPropertyName("havePhone")]
        public int? HavePhone { get; set; }
        /// <summary>
        /// 期望日期
        /// </summary>
        [JsonPropertyName("expectedDate")]
        public DateTime? ExpectedDate { get; set; }
        /// <summary>
        /// 承辦人清單
        /// </summary>
        public List<SRTaskOwnerModel> taskOwnerList { get; set; }
    }
}