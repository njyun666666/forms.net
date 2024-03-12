using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FormsNet.Models.File
{
    public class CKEditorUploadModel
    {
        public IFormFile upload { get; set; }
    }
    public class CKEditorViewModel
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string url { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CKEditorMessage error { get; set; }
    }
    public class CKEditorMessage
    {
        public string message { get; set; }
    }
}
