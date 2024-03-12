using FormsNet.DB.DBClass;
using FormsNet.DB.IDB;
using FormsNet.Models.File;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.DB
{
    public class BackEndV2DB_File : IBackEndV2DB_File
    {
        public readonly string DbName = "BackEndV2DB";
        public string str_conn;

        public IDBConnection _dBConnection;

        public BackEndV2DB_File(IDBConnection dBConnection)
        {
            _dBConnection = dBConnection;
            str_conn = _dBConnection.Connection(DbName);
        }

        public async Task<int> Insert(FileListModel model)
        {
            string sql = "INSERT INTO `TB_File_List`" +
                        " (`FileID`,`GroupID`,`OFileName`,`NFileName`,`Path`,`Size`,`UploadDate`,`UID`,`Status`)" +
                        " VALUES" +
                        " (@in_FileID, @in_GroupID, @in_OFileName, @in_NFileName, @in_Path, @in_Size, @in_UploadDate, @in_UID, @in_Status);";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_FileID", model.FileID, DbType.String);
            _params.Add("@in_GroupID", model.GroupID, DbType.String);
            _params.Add("@in_OFileName", model.OFileName, DbType.String);
            _params.Add("@in_NFileName", model.NFileName, DbType.String);
            _params.Add("@in_Path", model.Path, DbType.String);
            _params.Add("@in_Size", model.Size, DbType.Int64);
            _params.Add("@in_UploadDate", model.UploadDate, DbType.DateTime);
            _params.Add("@in_UID", model.UID, DbType.String);
            _params.Add("@in_Status", model.Status, DbType.Int16);

            return await AsyncDB.ExecuteAsync(str_conn, sql, _params);
        }
        public async Task<List<FileListViewModel>> GetFileList(string groupID)
        {
            string sql = "select f.*, a.Name as UserName from TB_File_List f join TB_Org_Account_Info a on f.UID=a.UID" +
                        " where f.GroupID = @in_groupID and f.Status in (0, 1) and f.DeleteDraft=0";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_groupID", groupID, DbType.String);

            return await AsyncDB.QueryAsync<FileListViewModel>(str_conn, sql, _params);
        }
        /// <summary>
        /// 取得表單全部附件
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public async Task<List<FileListViewModel>> GetFileList_ByFormID(string formID)
        {
            string sql = "select f.*, a.Name as UserName from TB_File_List f join TB_Org_Account_Info a on f.UID=a.UID" +
                        "  where f.GroupID in" +
                        " (" +
                        " 	select FileGroupID from TB_Form_List where FormID=@in_formID and Status=1" +
                        " 	union" +
                        "   select FileGroupID from TB_Sign_Log where FormID=@in_formID and Status=1" +
                        "  )" +
                        " and f.Status=1";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_formID", formID, DbType.String);

            return await AsyncDB.QueryAsync<FileListViewModel>(str_conn, sql, _params);
        }
        public async Task<int> DeleteFile(string uid, string fileID)
        {
            string sql = "UPDATE `TB_File_List` SET `DeleteDraft` = 1 WHERE `FileID` = @in_fileID and `UID`=@in_uid ";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String);
            _params.Add("@in_fileID", fileID, DbType.String);

            return await AsyncDB.ExecuteAsync(str_conn, sql, _params);
        }
        /// <summary>
        /// 檔案轉正式
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="uid"></param>
        /// <param name="fileGroupID"></param>
        /// <returns></returns>
        public async Task<int> FileToRelease(MySqlConnection conn, MySqlTransaction transaction, string uid, string fileGroupID, DateTime? UploadDate)
        {
            if (!UploadDate.HasValue)
            {
                UploadDate = DateTime.Now;
            }

            string sql = "UPDATE `TB_File_List` SET `Status` = 1 , `UploadDate`=@in_UploadDate WHERE `GroupID` = @in_groupID and `UID`=@in_uid and `Status`=0 and `DeleteDraft`=0 ;";

            // delete
            sql += " UPDATE `TB_File_List` SET `Status` = -1 WHERE `GroupID` = @in_groupID and `UID`=@in_uid and `DeleteDraft`=1 ;";

            DynamicParameters _params = new DynamicParameters();
            _params.Add("@in_uid", uid, DbType.String);
            _params.Add("@in_groupID", fileGroupID, DbType.String);
            _params.Add("@in_UploadDate", UploadDate, DbType.DateTime);

            return await conn.ExecuteAsync(sql, _params, transaction);
        }
    }
}
