using FormsNet.DB.DBClass;
using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;

namespace FormsNet.DB
{
	public interface IDB_Test
	{
		public Task<int> BackEndV2DBTest();
	}
	public class DB_Test : IDB_Test
	{
		private string backEndV2DB_conn;
		public DB_Test(IDBConnection dBConnection)
		{
			backEndV2DB_conn = dBConnection.Connection("BackEndV2DB");
		}
		public async Task<int> BackEndV2DBTest()
		{
			int result = 0;
			try
			{
				using (MySqlConnection conn = new MySqlConnection(backEndV2DB_conn))
				{
					if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
					result = 1;
				}
			}
			catch (Exception ex)
			{
				//throw ex;
			}
			return result;
		}

	}
}
