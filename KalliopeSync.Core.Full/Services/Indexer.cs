using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using KalliopeSync.Core.Models;

namespace KalliopeSync.Core.Services
{
    public class Indexer
    {
        private readonly string _accountName;
        private readonly string _accountKey;
        private readonly string _containerName;
        private readonly CloudStorageAccount _storageAccount;
        private readonly Dictionary<string, CloudBlockBlob> _cloudRepository;
        private readonly Dictionary<string, IndexItem> _index;
        private readonly string[] _patterns;

        public bool SimulationMode
        {
            get;
            set;
        }

        public Dictionary<string,IndexItem> Index
        {
            get
            {
                return _index;
            }
        }

        public Indexer(string userName, string accountName, string accountKey, string targetFolder)
        {
            this._accountName = accountName;
            this._accountKey = accountKey;
            this._containerName = userName;
            this._cloudRepository = new Dictionary<string, CloudBlockBlob>();
            this._index = new Dictionary<string, IndexItem>();

            if (accountName == "" || accountKey == "")
            {
                _storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            }
            else
            {
                var cloudConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", _accountName, _accountKey);
                _storageAccount = CloudStorageAccount.Parse(cloudConnectionString);
            }
            this._patterns = LoadPatterns(targetFolder);
        }

        public string GetBlobReferenceName(string fullFileName, string targetFolder)
        {
            string blobReferenceName = Path.GetFullPath(fullFileName).Replace(targetFolder, "").Replace(@"\",@"/");
            return blobReferenceName;
        }

        public IEnumerable<FileInfo> CreateUploadList(string targetFolder)
        {
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_containerName);

            foreach (IListBlobItem item in container.ListBlobs(null, true))
            {
                CloudBlockBlob blob = (CloudBlockBlob)item;
                Logging.Logger.Info(string.Format("Cloud List: Blob Name '{0}'", blob.Name));
                _cloudRepository[System.Net.WebUtility.HtmlDecode(blob.Name)] = blob;
            }

            var localRepository = GetLocalRepository(targetFolder, null);
            var uploadList = new List<FileInfo>();

            Console.WriteLine("Creating Upload List");
            Logging.Logger.Info("Creating Upload List");


            foreach (var item in localRepository)
            {
                string blobReferenceName = GetBlobReferenceName(item.Value.FullName, targetFolder);
                if (!_cloudRepository.ContainsKey(blobReferenceName))
                {
                    Logging.Logger.Info(string.Format("Added File to UploadList: {0}", blobReferenceName));
                    uploadList.Add(item.Value);
                    this.AddToIndex(blobReferenceName, item.Value, null, SyncStatus.UploadPending);
                }
                else if (_cloudRepository.ContainsKey(blobReferenceName))
                {
                    if (item.Value.Length != _cloudRepository[blobReferenceName].Properties.Length)
                    {
                        Logging.Logger.Info(string.Format("Added File to UploadList (changed): {0}, cloud {1}, local {2}", blobReferenceName, _cloudRepository[blobReferenceName].Properties.Length, item.Value.Length));
                        uploadList.Add(item.Value);
                        this.AddToIndex(blobReferenceName, item.Value, null, SyncStatus.UploadPending);
                    }
                    else
                    {
                        this.AddToIndex(blobReferenceName, item.Value, null, SyncStatus.InSync);
                    }
                }
            }
            Console.WriteLine("Creating Upload List COMPLETE --------------------------");
            Logging.Logger.Info("Creating Upload List COMPLETE ------------------------");
            return uploadList;
        }

        private void AddToIndex(string path, FileInfo fileInfo, CloudBlockBlob blob, SyncStatus status)
        {
            this.Index.Add(path, 
                new IndexItem
                {
                    Path = path,
                    File = fileInfo,
                    Blob = blob,
                    Status = status
                });

        }
        public IEnumerable<CloudBlockBlob> CreateDeleteList()
        {
            return null;
        }

        public Dictionary<string, FileInfo> GetLocalRepository(string folderName, Dictionary<string, FileInfo> localRepository )
        {            
            if (localRepository == null)
            {
                localRepository = new Dictionary<string, FileInfo>();
            }
            if (IsFolderIncluded(folderName, _patterns))
            {
                var directoryInfoList = Directory.EnumerateDirectories(folderName);
                foreach (var directory in directoryInfoList)
                {
                    localRepository = GetLocalRepository(directory, localRepository);
                }
                var filesList = Directory.EnumerateFiles(folderName);
                foreach (var file in filesList)
                {
                    if (IsFileIncluded(file, _patterns))
                    {
                        localRepository.Add(file, new FileInfo(file));
                        Logging.Logger.Info(string.Format("Added File to Repo: {0}", file));
                    }
                    else
                    {
                        Logging.Logger.Info(string.Format("Skipped File: {0}", file));
                    }
                }
            }
            else
            {
                Logging.Logger.Info(string.Format("Skipped Folder: {0}", folderName));
            }
            return localRepository;
        }

        public bool IsFolderIncluded(string folderName, string [] patterns)
        {
            bool isExcluded = false;
            if (!folderName.EndsWith(@"/"))
            {
                folderName += @"/";
            }
            for (int i = 0; i < patterns.Length; i++)
            {
                if (folderName.Like(patterns[i]))
                {
                    isExcluded = true;
                    break;
                }
            }
            return !isExcluded;
        }

        public bool IsFileIncluded(string fileName, string [] patterns)
        {
            bool isExcluded = false;
            for (int i = 0; i < patterns.Length; i++)
            {
                if (fileName.Like(patterns[i]))
                {
                    isExcluded = true;
                    break;
                }
            }
            return !isExcluded;
        }

        public string []  LoadPatterns(string directory)
        {
            List<string> patterns = new List<string>(System.IO.File.ReadLines(Path.Combine(directory, ".kpsignore")));
            string message = "Patterns used: ";
            patterns.ForEach((string s) =>
                {
                    message += s + ",";
                });
            Logging.Logger.Info(message.TrimEnd(','), null);
            return patterns.ToArray();
        }
    }
}

