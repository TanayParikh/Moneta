using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Data.Sqlite.Internal;
using Windows.Storage;
using System.IO;
using MonetaFMS.Models;
using System.Data;
using MonetaFMS.Interfaces;

namespace MonetaFMS.Services
{
    public class DBService : IDBService
    {
        string DBFileName = "MonetaDB.db";
        string DBPath => Path.Combine(ApplicationData.Current.LocalFolder.Path, DBFileName);
        
        string _dbConnectionString;
        public string DBConnectionString {
            get {
                if (string.IsNullOrEmpty(_dbConnectionString))
                {
                    SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder
                    {
                        DataSource = DBPath
                    };

                    _dbConnectionString = connectionStringBuilder.ConnectionString;
                }
                
                return _dbConnectionString;
            }
        }
        
        /// <summary>
        ///  Performs a backup of the DBFile
        /// </summary>
        /// <param name="deleteAfterBackup">DBFile to be deleted after backup (false)</param>
        public void BackupDB(bool deleteAfterBackup = false)
        {
            if (File.Exists(DBPath))
            {
                int backupNum = 0;

                while (File.Exists(DBPath + ++backupNum)) ;

                File.Copy(DBPath, DBPath + backupNum);

                if (deleteAfterBackup)
                    File.Delete(DBPath);
            }
        }

        const string SELECT_ROW_ID = ";SELECT last_insert_rowid();";
        
        public DBService()
        {
            SqliteEngine.UseWinSqlite3(); //Configuring library to use SDK version of SQLite

            VerifyConnection();
        }

        #region DatabaseConfiguration
        /// <summary>
        /// Ensures the SQLite connection can be made
        /// </summary>
        private void VerifyConnection()
        {
            try
            {
                if (!File.Exists(DBPath) || (File.Exists(DBPath) && !VerifyTableIntegrity()))
                {
                    RestoreDB();
                }
            }
            catch (SqliteException)
            {
                RestoreDB();
            }
        }

        private bool VerifyTableIntegrity()
        {
            using (SqliteConnection db = new SqliteConnection(DBConnectionString))
            {
                db.Open();

                String tableCommand = "SELECT * FROM sqlite_master WHERE type='table';";
                SqliteCommand getTables = new SqliteCommand(tableCommand, db);

                var reader = getTables.ExecuteReader();
                List<string> tablesRead = new List<string>();

                while (reader.Read())
                {
                    tablesRead.Add(reader.GetString(1));
                }

                string[] expectedTables = Enum.GetNames(typeof(DBTables));
                string[] missingTables = expectedTables.Except(tablesRead).ToArray();

                if (missingTables.Length == 0)
                    return true;
                else if (missingTables.Length != expectedTables.Length)
                    throw new Exception($"Missing table in MonetaDB, {string.Join(", ", missingTables)}");
            }

            return false;
        }

        /// <summary>
        /// Performs the Update command and returns whether it was successful
        /// </summary>
        /// <param name="command">Update SQL command</param>
        /// <returns></returns>
        public bool UpdateValue(SqliteCommand command)
        {
            try
            {
                using (SqliteConnection db = new SqliteConnection(DBConnectionString))
                {
                    db.Open();
                    command.Connection = db;

                    return command.ExecuteNonQuery() != 0;
                }
            }
            catch (SqliteException)
            {
                return false;
            }
        }

        /// <summary>
        /// Performs the given insert command and returns the PK of the new element
        /// </summary>
        /// <param name="command">Insert SQL command</param>
        /// <returns></returns>
        public int InsertValue(SqliteCommand command)
        {
            using (SqliteConnection db = new SqliteConnection(DBConnectionString))
            {
                db.Open();
                command.Connection = db;
                
                command.CommandText += SELECT_ROW_ID;
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Restores the MonetaDB to installation fresh copy
        /// </summary>
        private void RestoreDB()
        {
            // Backup existing file if applicable
            BackupDB(true);

            string freshDBPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Resources/MonetaDB_DEMO_DATA.db"); //"Resources/MonetaDB.db");

            if (File.Exists(freshDBPath))
                File.Copy(freshDBPath, DBPath);
            else
                throw new Exception("Unable to restore database.");
        }
        #endregion
    }
}
