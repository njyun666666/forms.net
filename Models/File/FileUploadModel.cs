using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Models.File
{
    public class FileUploadModel
    {
        public string GroupID { get; set; }
        public List<IFormFile> File { get; set; }
    }
}
