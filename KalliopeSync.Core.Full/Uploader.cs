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
        private readonly Dictionary<string, CloudBlockBlob> _cloudRepository;
        private readonly Dictionary<string, FileInfo> _localRepository;

        public Uploader(string userName, string accountName, string accountKey)
        {
            this._accountName = accountName;
            this._accountKey = accountKey;
            this._containerName = userName;
            this._cloudRepository = new Dictionary<string, CloudBlockBlob>();
            this._localRepository = new Dictionary<string, FileInfo>();
        }

        public void Upload(string targetFolder)
        {
            var cloudConnectionString = string.Format ("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", _accountName, _accountKey);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cloudConnectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(_containerName);

            int currentCount = 0;
            foreach (IListBlobItem item in container.ListBlobs(null, true))
            {
                CloudBlockBlob blob = (CloudBlockBlob)item;
                Console.WriteLine("Cloud List: Blob {0}", blob.Name);

                _cloudRepository[blob.Name] = blob;
            }

            GetLocalRepository(targetFolder);

            var uploadList = CreateUploadList(targetFolder);
            var deleteList = CreateDeleteList();

            Upload(uploadList, container, targetFolder);
            Delete(deleteList);

        }

        private void Upload(IEnumerable<FileInfo> uploadList, CloudBlobContainer container, string targetFolder)
        {
            foreach (var fileInfo in uploadList)
            {
                string blobReferenceName = GetBlobReferenceName(fileInfo.FullName, targetFolder);
                var blockBlob = container.GetBlockBlobReference(blobReferenceName);
                Console.WriteLine("Upload List: File {0}, Blob Reference {1}", fileInfo.FullName, blobReferenceName);
                using (var fileStream = System.IO.File.OpenRead(fileInfo.FullName))
                {
                    Console.WriteLine("Uploading: File {0} to Blob Reference {1}", fileInfo.FullName, blobReferenceName);
                    blockBlob.UploadFromStream(fileStream);
                }                                
            }
        }

        private void Delete(IEnumerable<CloudBlockBlob> deleteList)
        {
            throw new NotImplementedException();
        }

        private string GetBlobReferenceName(string fullFileName, string targetFolder)
        {
            string blobReferenceName = Path.GetFullPath(fullFileName).Replace(targetFolder, "");
            return blobReferenceName;

        }

        private IEnumerable<FileInfo> CreateUploadList(string targetFolder)
        {
            List<FileInfo> uploadList = new List<FileInfo>();
            foreach (var item in _localRepository)
            {
                string blobReferenceName = GetBlobReferenceName(item.Value.FullName, targetFolder);
                if (!_cloudRepository.ContainsKey(blobReferenceName))
                {
                    Console.WriteLine("Added File to UploadList: {0}", blobReferenceName);
                    uploadList.Add(item.Value);
                }
                else if (_cloudRepository.ContainsKey(blobReferenceName))
                {
                    if (item.Value.Length != _cloudRepository[blobReferenceName].Properties.Length)
                    {
                        Console.WriteLine("Added File to UploadList (changed): {0}, cloud {1}, local {2}", blobReferenceName, _cloudRepository[item.Value.Name].Properties.Length, item.Value.Length);
                        uploadList.Add(item.Value);
                    }
                }
            }
            return uploadList;
        }

        private IEnumerable<CloudBlockBlob> CreateDeleteList()
        {
            return null;
        }


        private void GetLocalRepository(string folderName)
        {
            var directoryInfoList = Directory.EnumerateDirectories(folderName);
            foreach (var directory in directoryInfoList)
            {
                GetLocalRepository(directory);
            }
            var filesList = Directory.EnumerateFiles(folderName);
            foreach (var file in filesList)
            {
                _localRepository.Add(file, new FileInfo(file));
                Console.WriteLine("Added File to Repo: {0}", file);
            }                
        }
    }
}

