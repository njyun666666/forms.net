using FormsNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FormsNet.Models.File
{
    public class FileListModel
    {
        public string FileID { get; set; }
        public string GroupID { get; set; }
        public string OFileName { get; set; }
        public string NFileName { get; set; }
        public string Path { get; set; }
        public Int64 Size { get; set; }
        public DateTime UploadDate { get; set; }
        public string UID { get; set; }
        public Int16 Status { get; set; }
    }
    public class FileListViewModel : FileListModel
    {
        /// <summary>
        /// 上傳者姓名
        /// </summary>
        public string UserName { get; set; }
        public string SizeText
        {
            get
            {
                decimal _size = Size;
                string unit = "Bytes";

                // KB
                if (_size > 1024)
                {
                    _size = _size / 1024;
                    unit = "KB";
                }

                // MB
                if (_size > 1024)
                {
                    _size = _size / 1024;
                    unit = "MB";
                }

                return $"{Math.Round(_size, 2)} {unit}";
            }
        }
        [JsonIgnore]
        public string FileSite { get; set; }
        public string Url
        {
            get
            {
                return $"{FileSite}{Path}";
            }
        }

    }

    public class FileUploadResultModel
    {
        public List<FileListViewModel> list { get; set; }
        public string error_message { get; set; }
        public List<string> error_list { get; set; }
        public FileUploadResultModel()
        {
            list = new List<FileListViewModel>();
            error_list = new List<string>();
        }
    }

    public class GetFileListModel
    {
        public string FormID { get; set; }
        public string GroupID { get; set; }
    }

}
