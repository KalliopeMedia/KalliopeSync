using NUnit.Framework;
using System;
using KalliopeSync.Db;
using System.IO;
using Mono.Data.Sqlite;

namespace KalliopeSync.Tests
{
    [TestFixture()]
    public class TestSyncItemServices
    {
        [Test()]
        public void GetAll_None_Test()
        {
            string fileName = DateTime.Now.Ticks.ToString() + ".kpsdb";
            string folderName = DateTime.Now.Ticks.ToString();
            var provider = new Provider(fileName, folderName);

            ISyncItemServices services = new SyncItemServices(provider);
            var outputData = services.GetAll();
            Assert.AreEqual(outputData.Count, 0);
            File.Delete(provider.DbFile);
            Directory.Delete(folderName, true);

        }

        [Test()]
        public void GetAll_CountGtZero_Test()
        {
            string fileName = DateTime.Now.Ticks.ToString() + ".kpsdb";
            string folderName = DateTime.Now.Ticks.ToString();
            var provider = new Provider(fileName, folderName);

            using (var connection = provider.GetConnection())
            {
                connection.Open();
                using (var command = new SqliteCommand(connection))
                {
                    this.InsertDummyData(CreateDummyData(), command);
                    this.InsertDummyData(CreateDummyData(), command);
                    this.InsertDummyData(CreateDummyData(), command);
                    ISyncItemServices services = new SyncItemServices(provider);
                    var outputData = services.GetAll();
                    Assert.AreEqual(outputData.Count, 3);
                }
                connection.Close();
            }
            File.Delete(provider.DbFile);
            Directory.Delete(folderName, true);
        }

        [Test()]
        public void Create_All_Test()
        {
            string fileName = DateTime.Now.Ticks.ToString() + ".kpsdb";
            string folderName = DateTime.Now.Ticks.ToString();
            var provider = new Provider(fileName, folderName);

            var sampleData = CreateDummyData();
            ISyncItemServices services = new SyncItemServices(provider);
            var returnData = services.CreateSyncItem(sampleData);

            using (var connection = provider.GetConnection())
            {
                connection.Open();
                using (var command = new SqliteCommand(string.Format(
                    @"SELECT Id, FileKey, CloudAssetId,
                CloudLastUpdatedDate, CloudCreatedDate, IsDeleted,
                Size FROM {0} WHERE Id={1}", SyncItem.TableName, returnData.Id), connection))
                {
                    var reader = command.ExecuteReader();
                    SyncItem outputData = new SyncItem();

                    while (reader.Read())
                    {
                        outputData.Id = reader.GetInt64(0);
                        outputData.FileKey = reader.GetString(1);
                        outputData.CloudAssetId = reader.GetString(2);
                        outputData.CloudLastUpdated = new DateTime(reader.GetInt64(3));
                        outputData.CloudCreated = new DateTime(reader.GetInt64(4));
                        outputData.IsDeleted = reader.GetBoolean(5);
                        outputData.Size = reader.GetInt64(6);
                    }
                    Assert.AreEqual(outputData.Id, returnData.Id);
                    Assert.AreEqual(outputData.FileKey, returnData.FileKey);
                    Assert.AreEqual(outputData.CloudAssetId, returnData.CloudAssetId);
                    Assert.AreEqual(outputData.CloudLastUpdated, returnData.CloudLastUpdated);
                    Assert.AreEqual(outputData.CloudCreated, returnData.CloudCreated);
                    Assert.AreEqual(outputData.IsDeleted, returnData.IsDeleted);
                    Assert.AreEqual(outputData.Size, returnData.Size);
                }
                connection.Close();
            }
            File.Delete(provider.DbFile);
            Directory.Delete(folderName, true);
        }

        [Test()]
        public void Read_All_ById_Test()
        {
        }

        [Test()]
        public void Update_DateTime_Size_Test()
        {
        }

        [Test()]
        public void Delete_ById_Test()
        {
        }

        private SyncItem CreateDummyData()
        {
            var dummy = new SyncItem
            {
                    Id = DateTime.Now.Ticks,
                    FileKey = Guid.NewGuid().ToString(),
                    CloudAssetId = Guid.NewGuid().ToString(),
                    CloudCreated = DateTime.Now,
                    CloudLastUpdated = DateTime.Now,
                    IsDeleted = false,
                    Size = 1024
            };
            return dummy;
        }

        private void InsertDummyData(SyncItem dummyData, SqliteCommand cmd)
        {
            cmd.CommandText = string.Format(
                @"INSERT INTO {0}(
                        FileKey, 
                        CloudAssetId, 
                        CloudLastUpdatedDate, 
                        CloudCreatedDate,
                        IsDeleted,
                        Size) 
                        VALUES(
                        @FileKey, 
                        @CloudAssetId,
                        @CloudLastUpdated, 
                        @CloudCreated,
                        @IsDeleted,
                        @Size)", SyncItem.TableName);
            cmd.Prepare();

            cmd.Parameters.AddWithValue("@FileKey", dummyData.FileKey);
            cmd.Parameters.AddWithValue("@CloudAssetId", dummyData.CloudAssetId);
            cmd.Parameters.AddWithValue("@CloudLastUpdated", dummyData.CloudLastUpdated.Ticks);
            cmd.Parameters.AddWithValue("@CloudCreated", dummyData.CloudCreated.Ticks);
            cmd.Parameters.AddWithValue("@IsDeleted", dummyData.IsDeleted);
            cmd.Parameters.AddWithValue("@Size", dummyData.Size);


            cmd.ExecuteNonQuery();
        }
    }
}

