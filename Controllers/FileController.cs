using FormsNet.Common;
using FormsNet.Models.File;
using FormsNet.Services.IServices;
using FormsNet.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        IMyService _myService;
        IFileService _fileService;
        FileSettingModel fileSetting;
        string FileSizeError;

        public FileController(IMyService myService, IFileService fileService)
        {
            _myService = myService;
            _fileService = fileService;
            fileSetting = _myService.FileSetting();
            FileSizeError = $"檔案大小限制 {fileSetting.FileSizeLimit_MB} MB 以下";
        }
        [HttpPost]
        public async Task<IActionResult> CKEditorUpload([FromForm]CKEditorUploadModel model)
        {
            CKEditorViewModel result = new CKEditorViewModel();
            

            if (model.upload.Length > fileSetting.FileSizeLimit_Byte)
            {
                result.error = new CKEditorMessage() { message = FileSizeError };
                return Ok(result);
            }

            string root = fileSetting.Root;
            string relative_path = "/ckeditor";
            string save_path = $"{root}{relative_path}";
            string nFileName = Path.GetRandomFileName().Replace(".", "") + Path.GetExtension(model.upload.FileName);
            string filePath = $"{save_path}/{nFileName}";
            string fileSite = fileSetting.FileSite;
            string url = $"{fileSite}{relative_path}/{nFileName}";


            if (!Directory.Exists(save_path))
            {
                Directory.CreateDirectory(save_path);
            }

            using (var stream = System.IO.File.Create(filePath))
            {
                await model.upload.CopyToAsync(stream);
            }

            result.url = url;

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Upload([FromHeader] string uid, [FromForm] FileUploadModel model)
        {
            FileUploadResultModel result = new FileUploadResultModel();
            
            string root = fileSetting.Root;
            string fileSite = fileSetting.FileSite;
            DateTime now = DateTime.Now;


            foreach (IFormFile file in model.File)
            {

                try
                {
                    if (file.Length > fileSetting.FileSizeLimit_Byte)
                    {
                        result.error_list.Add(file.FileName);
                        result.error_message = FileSizeError;
                    }
                    else
                    {
                        string FileID = Guid.NewGuid().ToString();
                        string nFileName = file.FileName ;//FileID + Path.GetExtension(file.FileName);

                        string relative_path = $"/{model.GroupID}/{FileID}";
                        string save_path = $"{root}/{relative_path}";
                        string filePath = $"{save_path}/{nFileName}";

                        string url = $"{fileSite}{relative_path}/{nFileName}";


                        if (!Directory.Exists(save_path))
                        {
                            Directory.CreateDirectory(save_path);
                        }


                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }



                        FileListModel fileData = new FileListModel
                        {
                            FileID = FileID,
                            GroupID = model.GroupID,
                            OFileName = file.FileName,
                            NFileName = nFileName,
                            Path = $"{relative_path}/{nFileName}",
                            Size = file.Length,
                            UploadDate = now,
                            UID = uid,
                            Status = 0
                        };

                        int insert_result = await _fileService.Insert(fileData);


                        if (insert_result > 0)
                        {
                            result.list.Add(new FileListViewModel()
                            {
                                FileID = fileData.FileID,
                                GroupID = fileData.GroupID,
                                OFileName = fileData.OFileName,
                                NFileName = fileData.NFileName,
                                Path = fileData.Path,
                                Size = fileData.Size,
                                UploadDate = fileData.UploadDate,
                                UID = fileData.UID,
                                Status = fileData.Status
                            });
                        }
                        else
                        {
                            result.error_list.Add(file.FileName);
                        }

                    }

                }
                catch (Exception ex)
                {
                    result.error_list.Add(file.FileName);
                }

            }


            result.list.ForEach(x =>
            {
                x.FileSite = fileSetting.FileSite;
            });


            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetFileList(GetFileListModel model)
        {
            return Ok(await _fileService.GetFileList(model));
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromHeader] string uid, FileDeleteModel model)
        {
            int result = await _fileService.DeleteFile(uid, model.FileID);

            if (result == 0)
            {
                return Ok(new FailResponse());
            }

            return Ok(new OKResponse());
        }


    }
}
