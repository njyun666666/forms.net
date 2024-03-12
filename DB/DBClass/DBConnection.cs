using FormsNet.Helper;
using Microsoft.Extensions.Configuration;

namespace FormsNet.DB.DBClass
{
	public interface IDBConnection
	{
		public string Connection(string dbName);
	}
	public class DBConnection : IDBConnection
	{
		private IConfiguration _config;
		public DBConnection(IConfiguration config)
		{
			_config = config;

		}

		public string Connection(string dbName)
		{
#if DEBUG
			return _config["DBSetting:" + dbName];
#endif
			//return _config["DBSetting:" + dbName];
			return AccessSecretVersion.Get("citric-banner-315704", "DBSetting-" + dbName);

		}


	}
}
