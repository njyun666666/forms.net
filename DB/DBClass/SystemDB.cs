using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB.DBClass
{
    public class SystemDB
    {
        private const string ReadOnlySession = "set session transaction isolation level read uncommitted";
        private const string ReadCommitSession = "set session transaction isolation level read committed";


        static public void testconnect(string connectionstr)
        {
            using (var conn = new MySqlConnection(connectionstr))
            {
                conn.Open();
                conn.Close();
            }
        }

        static public List<T> Query<T>(string connStr, string sqlStatement, object param = null,
            IDbTransaction trans = null, CommandType type = CommandType.Text)
        {

            List<T> result;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    conn.Execute(ReadOnlySession);
                    result = conn.Query<T>(sqlStatement, param, trans, commandType: type).AsList<T>();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return result;
        }



        static public T SingleQuery<T>(string connStr, string sqlStatement, object param = null, IDbTransaction trans = null,
            CommandType type = CommandType.Text, bool NeedCommitData = false)
        {

            T result;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    if (NeedCommitData)
                    {
                        conn.Execute(ReadCommitSession);
                    }
                    else
                    {
                        conn.Execute(ReadOnlySession);
                    }
                    result = conn.Query<T>(sqlStatement, param, trans, commandType: type).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return result;
        }

        static public int Insert(string connStr, string sqlStatement, object param = null, IDbTransaction trans = null, bool DoTransaction = true)
        {
            int result = 0;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                if (DoTransaction)
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            result = conn.Execute(sqlStatement, param, transaction);
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
                else
                {
                    try
                    {
                        result = conn.Execute(sqlStatement, param);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

            }
            return result;
        }

        static public int Update(string connStr, string sqlStatement, object param = null, IDbTransaction trans = null, bool DoTransaction = true)
        {
            int result = 0;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                if (DoTransaction)
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            result = conn.Execute(sqlStatement, param, transaction);
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
                else
                {
                    try
                    {
                        result = conn.Execute(sqlStatement, param);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return result;

        }

        static public T InsertAndQuery<T>(string connStr, string sqlStatement, object param = null, IDbTransaction trans = null)
        {
            T result;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        result = conn.ExecuteScalar<T>(sqlStatement, param, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }

            return result;
        }
    }
}
