using System;
using System.IO;
using System.Configuration;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;

namespace KalliopeSync.Core.Services
{
    public class Downloader
    {
        private readonly string _accountName;
        private readonly string _accountKey;
        private readonly string _containerName;

        public bool SimulationMode
        {
            get;
            set;
        }

        public Downloader(string userName, string accountName, string accountKey)
        {
            this._accountName = accountName;
            this._accountKey = accountKey;
            this._containerName = userName;
        }



        public void DownloadAll(string targetFolder, int maxCount = 0)
        {
            var cloudConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", _accountName, _accountKey);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cloudConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(_containerName);

            int currentCount = 0;
            Console.WriteLine("Processing Downloads...");
            Logging.Logger.Info(string.Format("Processing Downloads-------------------- Account: {0} ; Key: {1} ; Container: {2}", _accountName, _accountKey, _containerName));

            foreach (IListBlobItem item in container.ListBlobs(null, true))
            {   
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    if (currentCount < maxCount || maxCount == -1)
                    {
                        currentCount = DownloadBlob(blob, targetFolder, currentCount);
                    }
                    else
                    {
                        Logging.Logger.Info(string.Format("Skipping {0} - size {1} because of maxCount - m={2}", blob.Name, blob.Properties.Length, maxCount));
                    }
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;
                    Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);
                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;
                    Console.WriteLine("Directory: {0}", directory.Uri);
                }
            }
            Console.WriteLine($"Processing Downloads COMPLETE, {currentCount} file(s) to downloaded");
            Logging.Logger.Info($"Processing Downloads COMPLETE, {currentCount} file(s) to downloaded");

        }

        private int DownloadBlob(CloudBlockBlob blockBlob, string targetFolder, int currentCount)
        {
            string decodedBlockBlobName = System.Net.WebUtility.HtmlDecode(blockBlob.Name).TrimStart(Path.DirectorySeparatorChar);
            string targetFileName = string.Format("{0}{1}{2}", targetFolder, Path.DirectorySeparatorChar.ToString(), decodedBlockBlobName);
            Logging.Logger.Info(string.Format("Target Folder: {0} Target file: {1} ", targetFolder, targetFileName));

            string tempName = Path.Combine(Path.Combine(targetFolder, Guid.NewGuid().ToString()));
            if (!File.Exists(targetFileName))
            {
                Logging.Logger.Info(string.Format("Local file {0} doesn't exist, download size {1}.", targetFileName, blockBlob.Properties.Length));
                currentCount++;
                if (!SimulationMode)
                {
                    var outFolder = Path.GetDirectoryName(targetFileName);
                    if(!Directory.Exists(outFolder)){
                        Directory.CreateDirectory(outFolder);
                    }
                    using (var fileStream = System.IO.File.OpenWrite(targetFileName))
                    {
                        blockBlob.DownloadToStream(fileStream);
                    }
                    File.SetCreationTime(targetFileName, blockBlob.Properties.LastModified.Value.DateTime);
                }
                else
                {
                    Logging.Logger.Info(string.Format("Download {0} to {1}", blockBlob.Name, targetFolder));
                }
            }
            else
            {
                FileInfo info = new FileInfo(targetFileName);
                if (blockBlob.Properties.Length != info.Length)
                {
                    if (blockBlob.Properties.LastModified.HasValue && blockBlob.Properties.LastModified.Value.DateTime > info.LastWriteTime)
                    {
                        Logging.Logger.Info(string.Format("Local file size: {0}, online file size: {1} Blob Updated: {2} FileUpdated: {3} Downloading...", info.Length, blockBlob.Properties.Length, blockBlob.Properties.LastModified.Value.DateTime, info.LastWriteTime));
                        currentCount++;
                        if (!SimulationMode)
                        {
                            using (var fileStream = System.IO.File.OpenWrite(tempName))
                            {
                                blockBlob.DownloadToStream(fileStream);
                            }
                            File.Delete(targetFileName);
                            File.Move(tempName, targetFileName);
                            File.SetCreationTime(targetFileName, blockBlob.Properties.LastModified.Value.DateTime);
                        }
                        else
                        {
                            Logging.Logger.Info(string.Format("Download {0} to {1}", blockBlob.Name, targetFolder));
                        }
                    }
                    else
                    {
                        Logging.Logger.Info(string.Format("Local file size: {0}, online file size: {1} - Could not determine last updated, local wins.", info.Length, blockBlob.Properties.Length));
                    }
                }
                else
                {
                    Logging.Logger.Info(string.Format("Local file size: {0}, online file size: {1} - Skipping...", info.Length, blockBlob.Properties.Length));
                }
            }
            return currentCount;
        }
    }
}

