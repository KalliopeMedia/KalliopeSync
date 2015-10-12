using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;

namespace KalliopeSync.Core.Full
{
    public class VirtualFileSystem
    {
        private readonly string _accountName;
        private readonly string _accountKey;
        private readonly string _containerName;

        public VirtualFileSystem(string accountName, string accountKey, string containerName)
        {
            _accountKey = accountKey;
            _accountName = accountName;
            _containerName = containerName;
        }

        public bool Move(string target)
        {
            var cloudConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", _accountName, _accountKey);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cloudConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(_containerName);

            Console.WriteLine("Processing Move...");
            Logging.Logger.Info("Processing Move--------------------");

            List<CloudBlockBlob> deleteItems = new List<CloudBlockBlob>();
            List<Task> copyTasks = new List<Task>();
            foreach (IListBlobItem item in container.ListBlobs(null, true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    deleteItems.Add(blob);
                    string targetBlobName = Path.Combine(target, blob.Name);
                    var targetBlob = container.GetBlobReference(targetBlobName);
                    Logging.Logger.Info(string.Format("Copying file {0} at {1}", targetBlobName, targetBlob.Uri));
                    copyTasks.Add(targetBlob.StartCopyAsync(blob.Uri));
                }
            }
            Console.WriteLine("Initiated Copy");
            Task.WhenAll(copyTasks.ToArray()).ContinueWith((Task arg) =>
                {
                    Console.WriteLine("Completed Copy... starting delete");
                    foreach (CloudBlockBlob item in deleteItems)
                    {
                        Logging.Logger.Info(string.Format("Deleting file at {0}", item.Uri));
                        item.Delete();
                    }
                }).Wait();
            Logging.Logger.Info(string.Format("Completed Move All"));
            Console.WriteLine(string.Format("Completed Copy All"));
            Console.ReadLine();
            return true;
        }
    }
}

