using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Game
{
    public class GameInfoModel
    {
        public string AppID { set; get; }
        public string AppName { set; get; }
        public string Area { set; get; }
        public int? TimeZone { set; get; }
        public int Status { set; get; }
        public DateTime CreateDate { set; get; }
        public DateTime? UpdateDate { set; get; }
        public string Editor { set; get; }
    }

    public class GameInfoAddModel : GameInfoModel
    {
        public bool isAdd { get; set; }
    }

    public class GameInfoGetAuthModel
    {
        public int MenuID { get; set; }
    }
}
