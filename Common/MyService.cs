using FormsNet.Models.File;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Common
{
    public interface IMyService
    {
        public string Google_client_id();
        public string BackEndV2_Key();
        public FileSettingModel FileSetting();
        public string AppsettingName();
    }
    public class MyService : IMyService
    {
        private IConfiguration _config;
        public MyService(IConfiguration config)
        {
            _config = config;
        }

        public string Google_client_id()
        {
            return _config["Google_client_id"];
        }
        public string BackEndV2_Key()
        {
            return _config["BackEndV2_Key"];
        }
        public FileSettingModel FileSetting()
        {
            return new FileSettingModel()
            {
                Root = _config["FileSetting:Root"],
                FileSite = _config["FileSetting:FileSite"],
                FileSizeLimit_MB = Convert.ToInt32(_config["FileSetting:FileSizeLimit_MB"]),
                FileSizeLimit_Byte = Convert.ToInt32(_config["FileSetting:FileSizeLimit_MB"]) * 1024 * 1024
            };
        }
        public string AppsettingName()
		{
            return _config["AppsettingName"];
		}
    }
}
