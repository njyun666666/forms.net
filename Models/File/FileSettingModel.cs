using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.File
{
    public class FileSettingModel
    {
        public string Root { get; set; }
        public string FileSite { get; set; }
        public int FileSizeLimit_MB { get; set; }
        /// <summary>
        /// Unit: Byte
        /// </summary>
        public int FileSizeLimit_Byte { get; set; }
    }
}
