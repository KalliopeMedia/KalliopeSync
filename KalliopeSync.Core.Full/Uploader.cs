using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;

namespace KalliopeSync.Core.Full
{
    public class Uploader
    {
        private readonly string _accountName;
        private readonly string _accountKey;
        private readonly string _containerName;
        private readonly Dictionary<string, CloudBlockBlob> _dictionary;

        public Uploader(string userName, string accountName, string accountKey)
        {
            this._accountName = accountName;
            this._accountKey = accountKey;
            this._containerName = userName;
        }

        public void Upload(string targetFolder)
        {
            var cloudConnectionString = string.Format ("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", _accountName, _accountKey);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cloudConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(_containerName);

            int currentCount = 0;
            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                CloudBlockBlob blob = (CloudBlockBlob)item;
                _dictionary[blob.Name] = blob;
            }

            // TODO: Identify files in local system not in Cloud and upload them
            using (var fileStream = System.IO.File.OpenRead(@"path\myfile"))
            {
                //blockBlob.UploadFromStream(fileStream);
            }            
            
        }
    }
}

