using FormsNet.Common;
using FormsNet.DB.IDB;
using FormsNet.Models.File;
using FormsNet.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services
{
    public class FileService : IFileService
    {
        IBackEndV2DB_File db_File;
        IMyService _myService;
        public FileService(IBackEndV2DB_File backEndV2DB_File, IMyService myService)
        {
            db_File = backEndV2DB_File;
            _myService = myService;
        }

        public async Task<int> Insert(FileListModel model)
        {
            return await db_File.Insert(model);
        }

        public async Task<List<FileListViewModel>> GetFileList(GetFileListModel model)
        {
            List<FileListViewModel> list = new List<FileListViewModel>();

            if (!string.IsNullOrWhiteSpace(model.GroupID))
            {
                list = await db_File.GetFileList(model.GroupID);
            }
            else
            {
                list = await db_File.GetFileList_ByFormID(model.FormID);
            }

            string FileSite = _myService.FileSetting().FileSite;

            list.ForEach(x =>
            {
                x.FileSite = FileSite;
            });

            return list;
        }
        public async Task<int> DeleteFile(string uid, string fileID)
        {
            return await db_File.DeleteFile(uid, fileID);
        }
    }
}
