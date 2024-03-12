using FormsNet.Models.File;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB.IDB
{
    public interface IBackEndV2DB_File
    {
        public Task<int> Insert(FileListModel model);
        public Task<List<FileListViewModel>> GetFileList(string groupID);
        public Task<List<FileListViewModel>> GetFileList_ByFormID(string formID);
        public Task<int> DeleteFile(string uid, string fileID);
        public Task<int> FileToRelease(MySqlConnection conn, MySqlTransaction transaction, string uid, string fileGroupID, DateTime? UploadDate);
    }
}
