using Microsoft.Data.Sqlite;

namespace MonetaFMS.Interfaces
{
    public interface IDBService
    {
        string DBConnectionString { get; }
        
        /// <summary>
        ///  Performs a backup of the DBFile
        /// </summary>
        /// <param name="deleteAfterBackup">DBFile to be deleted after backup (false)</param>
        void BackupDB(bool deleteAfterBackup = false);

        /// <summary>
        /// Wrapper for an insert command which executes command
        /// and returns the PK.
        /// </summary>
        /// <param name="command">Insert SQL command</param>
        /// <returns>PK of the new element</returns>
        int InsertValue(SqliteCommand command);

        /// <summary>
        /// Wrapper for an update command which executes command
        /// and returns whether it was successful. 
        /// </summary>
        /// <param name="command">Update SQL command</param>
        /// <returns>Success / Failure of DB Update</returns>
        bool UpdateValue(SqliteCommand command);
    }
}
