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
            Logging.Logger.Info("Processing Moving--------------------");

            Dictionary<string, CloudBlockBlob> deleteItems = new Dictionary<string, CloudBlockBlob>();
            foreach (IListBlobItem item in container.ListBlobs(null, true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    var targetBlob = container.GetBlobReference(Path.Combine(target, blob.Name));
                    Task<string> copy = targetBlob.StartCopyAsync(blob.Uri);
                    Logging.Logger.Info(string.Format("Beginning Move {0} to {1}", blob.Name, targetBlob.Name));
                    Console.WriteLine(string.Format("Beginning Move {0} to {1}", blob.Name, targetBlob.Name));
                    copy.ContinueWith((Task<string> returnValue) =>
                        {
                            if(returnValue.Status == TaskStatus.RanToCompletion)
                            {
                                Logging.Logger.Info(string.Format("Moved {0} to {1}: {2}", blob.Name, targetBlob.Name, returnValue.Result));
                                Console.WriteLine(string.Format("Moved {0} to {1}: {2}", blob.Name, targetBlob.Name, returnValue.Result));
                                blob.Delete(DeleteSnapshotsOption.IncludeSnapshots);
                                deleteItems.Add(blob.Name, blob);
                            }
                        });
                }
            }
            Console.ReadLine();
            //foreach (var deleteItem in deleteItems)
            //{
                //Logging.Logger.Info(string.Format("Deleting {0} ", deleteItem.Value.Name));
                //Console.WriteLine(string.Format("Deleting {0} ", deleteItem.Value.Name));
            //}
            return true;
        }
    }
}

