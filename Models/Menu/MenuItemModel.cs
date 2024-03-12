using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FormsNet.Models.Menu
{
    public class MenuItemModel
    {
        [JsonIgnore]
        public int id { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string link { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<MenuItemModel> children { get; set; }
    }
}


