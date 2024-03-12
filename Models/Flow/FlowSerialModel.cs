using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.Flow
{
    public class FlowSerialModel
    {
        public string FormID { get; set; }
        public string FormClass { get; set; }
        public DateTime Date { get; set; }
        public int YID { get; set; }
        public int MID { get; set; }
        public int DID { get; set; }
        public int TID { get; set; }
    }
}
