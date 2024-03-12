using FormsNet.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices
{
    public interface IFileService
    {
        public Task<int> Insert(FileListModel model);
        public Task<List<FileListViewModel>> GetFileList(GetFileListModel model);
        public Task<int> DeleteFile(string uid, string fileID);
    }
}
