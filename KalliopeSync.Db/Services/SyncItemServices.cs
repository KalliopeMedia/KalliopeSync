using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;


namespace KalliopeSync.Db
{
    public class SyncItemServices
    {
        Provider _provider;

        public SyncItemServices(Provider dataProvider)
        {
            _provider = dataProvider;
        }

        public IList<SyncItem> GetAll()
        {
            List<SyncItem> returnData = new List<SyncItem>();
            using (var conn = _provider.GetConnection())
            {
                using (SqliteCommand command = 
                    new SqliteCommand(
                        string.Format(
                            @"SELECT Id, FileKey, CloudAssetId,
                CloudLastUpdatedDate, CloudCreatedDate, IsDeleted,
                Size FROM {0}", SyncItem.TableName), conn))
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SyncItem item = new SyncItem();
                        item.Id = reader.GetInt32(0);
                        item.FileKey = reader.GetString(1);
                        item.CloudAssetId = reader.GetString(2);
                        item.CloudLastUpdated = new DateTime(reader.GetInt64(3));
                        item.CloudCreated = new DateTime(reader.GetInt32(4));
                        item.IsDeleted = reader.GetBoolean(5);
                    }
                }
            }
            return returnData;
        }
    }
}

