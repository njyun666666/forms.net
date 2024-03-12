using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB.DBClass
{
    public class AsyncDB
    {
        private const string ReadOnlySession = "set session transaction isolation level read uncommitted";
        private const string ReadCommitSession = "set session transaction isolation level read committed";


        public static void testconnect(string connectionstr)
        {
            using (var conn = new MySqlConnection(connectionstr))
            {
                conn.Open();
                conn.Close();
            }
        }

        public static async Task<List<T>> QueryAsync<T>(string connStr, string sqlStatement, object param = null,
            IDbTransaction trans = null, CommandType type = CommandType.Text)
        {

            List<T> result;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
                    await conn.ExecuteAsync(ReadOnlySession);
                    result = (await conn.QueryAsync<T>(sqlStatement, param, trans, commandType: type)).AsList<T>();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return result;
        }



        public static async Task<T> QueryFirstOrDefaultAsync<T>(string connStr, string sqlStatement, object param = null, IDbTransaction trans = null,
            CommandType type = CommandType.Text, bool NeedCommitData = false)
        {

            T result;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) await conn.OpenAsync();

                    if (NeedCommitData)
                    {
                        await conn.ExecuteAsync(ReadCommitSession);
                    }
                    else
                    {
                        await conn.ExecuteAsync(ReadOnlySession);
                    }
                    result = await conn.QueryFirstOrDefaultAsync<T>(sqlStatement, param, trans, commandType: type);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return result;
        }

        public static async Task<int> ExecuteAsync(string connStr, string sqlStatement, object param = null, IDbTransaction trans = null, bool DoTransaction = true)
        {
            int result = 0;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
                if (DoTransaction)
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            result = await conn.ExecuteAsync(sqlStatement, param, transaction);
                            await transaction.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            throw ex;
                        }
                    }
                }
                else
                {
                    try
                    {
                        result = await conn.ExecuteAsync(sqlStatement, param);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

            }
            return result;
        }

        public static async Task<T> ExecuteScalarAsync<T>(string connStr, string sqlStatement, object param = null, IDbTransaction trans = null)
        {
            T result;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        result = await conn.ExecuteScalarAsync<T>(sqlStatement, param, transaction);
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            }

            return result;
        }
    }
}
