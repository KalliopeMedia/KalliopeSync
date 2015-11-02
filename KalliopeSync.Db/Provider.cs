using System;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;


namespace KalliopeSync.Db
{
    public class Provider
    {
        public string ConnectionString
        {
            get;
            private set;
        }

        public string RootFolder
        {
            get;
            private set;
        }

        public string Type {
            get;
            private set;
        }

        public string DbFile
        {
            get;
            private set;
        }

        public Provider(string dbName= "kalliopesync.kpsdb", string rootFolder="")
        {
            this.RootFolder = rootFolder;
            if (!string.IsNullOrEmpty(RootFolder) && !Directory.Exists(RootFolder))
            {
                Directory.CreateDirectory(RootFolder);
            }             
            string dbFile = Path.Combine (RootFolder, dbName);
            this.DbFile = dbFile;
            string connectionString = string.Format("Data Source={0}", dbFile);
            Console.WriteLine(dbFile);
            Console.WriteLine(connectionString);
            this.ConnectionString = connectionString;
            bool exists = File.Exists (DbFile);

            if (!exists)
            {
                this.InitDb();
            }
            else
            {
                this.VerifyDb();
            }
                
        }

        public bool VerifyDb()
        {
            using (var conn = new SqliteConnection(ConnectionString))
            {
                conn.Open();
                var schema = conn.GetSchema("Tables");
                Console.WriteLine("Tables = " + schema.Rows.Count);
                conn.Close();
                return schema.Rows.Count == 2;
            }
        }

        private bool InitDb()
        {
            SqliteConnection.CreateFile (DbFile);

            var conn = new SqliteConnection(ConnectionString);

            var commands = new[] {
                @"CREATE TABLE SyncItem(
                    Id INTEGER PRIMARY KEY, 
                    FileKey TEXT, 
                    CloudAssetId TEXT,
                    CloudLastUpdatedDate INTEGER,
                    CloudCreatedDate INTEGER,
                    IsDeleted BOOLEAN,
                    Size INTEGER)",
                @"CREATE TABLE SyncVersion(
                    Id INTEGER PRIMARY KEY,
                    Version INTEGER,
                    VersionReleaseDate INTEGER,
                    LastUpdatedDate INTEGER,
                    IsActive BOOLEAN
                )",
                @"UPDATE SyncVersion SET IsActive = 0",
                string.Format(@"INSERT INTO SyncVersion (Version, VersionReleaseDate, LastUpdatedDate, IsActive) VALUES (1, {0}, {1}, 1)", DateTime.Now.Ticks, DateTime.Now.Ticks)
            };
            conn.Open ();
            foreach (var cmd in commands) {
                using (var c = conn.CreateCommand()) {
                    c.CommandText = cmd;
                    c.CommandType = CommandType.Text;
                    c.ExecuteNonQuery ();
                }
            }
            conn.Close ();
            return true;
        }

        /// <summary>
        /// Creates a new connection using the Provider details
        /// </summary>
        /// <returns>The a SqliteConnection instance</returns>
        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(ConnectionString);
        }
    }
}

