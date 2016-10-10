using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;


namespace KalliopeSync.Db
{
    public class SyncItemServices : ISyncItemServices
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
                conn.Open();
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
                        item.CloudCreated = new DateTime(reader.GetInt64(4));
                        item.IsDeleted = reader.GetBoolean(5);
                        returnData.Add(item);
                    }
                }
                conn.Close();
            }
            return returnData;
        }

        public SyncItem GetById(long id)
        {
            SyncItem item = new SyncItem();
            using (var conn = _provider.GetConnection())
            {
                conn.Open();
                using (SqliteCommand command = 
                    new SqliteCommand(
                        string.Format(
                            @"SELECT Id, FileKey, CloudAssetId,
                CloudLastUpdatedDate, CloudCreatedDate, IsDeleted,
                Size FROM {0} WHERE Id={1}", SyncItem.TableName, id), conn))
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        item.Id = reader.GetInt64(0);
                        item.FileKey = reader.GetString(1);
                        item.CloudAssetId = reader.GetString(2);
                        item.CloudLastUpdated = new DateTime(reader.GetInt64(3));
                        item.CloudCreated = new DateTime(reader.GetInt64(4));
                        item.IsDeleted = reader.GetBoolean(5);
                        item.Size = reader.GetInt64(6);
                    }
                }
                conn.Close();
            }
            return item;
        }

        public SyncItem Save(SyncItem item)
        {
            if (item.Id < 0)
            {
                return CreateSyncItem(item);
            }
            else
            {
                return UpdateSyncItem(item);
            }
        }

        public SyncItem CreateSyncItem(SyncItem newItem)
        {
            using(SqliteConnection con = _provider.GetConnection())
            {
                con.Open();

                using (SqliteCommand cmd = new SqliteCommand(con)) 
                {
                    cmd.CommandText = string.Format(
                        @"INSERT INTO {0}(
                        Id,
                        FileKey, 
                        CloudAssetId, 
                        CloudLastUpdatedDate, 
                        CloudCreatedDate,
                        IsDeleted,
                        Size) 
                        VALUES(
                        @Id,
                        @FileKey, 
                        @CloudAssetId,
                        @CloudLastUpdatedDate, 
                        @CloudCreatedDate,
                        @IsDeleted,
                        @Size)", SyncItem.TableName);
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@Id", newItem.Id);
                    cmd.Parameters.AddWithValue("@FileKey", newItem.FileKey);
                    cmd.Parameters.AddWithValue("@CloudAssetId", newItem.CloudAssetId);
                    cmd.Parameters.AddWithValue("@CloudLastUpdatedDate", newItem.CloudLastUpdated.Ticks);
                    cmd.Parameters.AddWithValue("@CloudCreatedDate", newItem.CloudCreated.Ticks);
                    cmd.Parameters.AddWithValue("@IsDeleted", newItem.IsDeleted);
                    cmd.Parameters.AddWithValue("@Size", newItem.Size);


                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            return newItem;
        }

        public SyncItem UpdateSyncItem(SyncItem updatedItem)
        {
            using(SqliteConnection con = _provider.GetConnection())
            {
                con.Open();

                using (SqliteCommand cmd = new SqliteCommand(con)) 
                {
                    cmd.CommandText = string.Format(@"UPDATE {0} SET 
                        FileKey = @FileKey, 
                        CloudAssetId = @CloudAssetId, 
                        CloudLastUpdatedDate = @CloudLastUpdatedDate, 
                        CloudCreatedDate = @CloudCreatedDate, 
                        IsDeleted = @IsDeleted,
                        Size = @Size WHERE 
                        Id = {1}", SyncItem.TableName, updatedItem.Id);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@FileKey", updatedItem.FileKey);
                    cmd.Parameters.AddWithValue("@CloudAssetId", updatedItem.CloudAssetId);
                    cmd.Parameters.AddWithValue("@CloudLastUpdatedDate", updatedItem.CloudLastUpdated.Ticks);
                    cmd.Parameters.AddWithValue("@CloudCreatedDate", updatedItem.CloudCreated.Ticks);
                    cmd.Parameters.AddWithValue("@IsDeleted", updatedItem.IsDeleted);
                    cmd.Parameters.AddWithValue("@Size", updatedItem.Size);
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            return updatedItem;
        }
    
        public bool DeleteSyncItem(long id)
        {
            throw new NotImplementedException("DeleteSyncItem not implemented");
        }
    }
}

