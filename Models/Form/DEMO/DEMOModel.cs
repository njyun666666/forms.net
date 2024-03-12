using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FormsNet.Models.Form.DEMO
{
    public class DEMOModel : FormListModel
    {
        /// <summary>
        /// AAA欄位
        /// </summary>
        [JsonPropertyName("aaa")]
        public string AAA { get; set; }
        /// <summary>
        /// BBB欄位
        /// </summary>
        [JsonPropertyName("bbb")]
        public string BBB { get; set; }
    }
}
