using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;

namespace KalliopeSync.Core.Full
{
    public class Indexer
    {
        private readonly string _accountName;
        private readonly string _accountKey;
        private readonly string _containerName;
        private readonly CloudStorageAccount _storageAccount;
        private readonly Dictionary<string, CloudBlockBlob> _cloudRepository;
        private readonly string[] _patterns;

        public bool SimulationMode
        {
            get;
            set;
        }

        public Indexer(string userName, string accountName, string accountKey, string targetFolder)
        {
            this._accountName = accountName;
            this._accountKey = accountKey;
            this._containerName = userName;
            this._cloudRepository = new Dictionary<string, CloudBlockBlob>();

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

        private string GetBlobReferenceName(string fullFileName, string targetFolder)
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
                _cloudRepository[blob.Name] = blob;
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
                }
                else if (_cloudRepository.ContainsKey(blobReferenceName))
                {
                    if (item.Value.Length != _cloudRepository[blobReferenceName].Properties.Length)
                    {
                        Logging.Logger.Info(string.Format("Added File to UploadList (changed): {0}, cloud {1}, local {2}", blobReferenceName, _cloudRepository[item.Value.Name].Properties.Length, item.Value.Length));
                        uploadList.Add(item.Value);
                    }
                }
            }
            Console.WriteLine("Creating Upload List COMPLETE --------------------------");
            Logging.Logger.Info("Creating Upload List COMPLETE ------------------------");
            return uploadList;
        }

        private IEnumerable<CloudBlockBlob> CreateDeleteList()
        {
            return null;
        }


        private Dictionary<string, FileInfo> GetLocalRepository(string folderName, Dictionary<string, FileInfo> localRepository )
        {            
            if (localRepository == null)
            {
                localRepository = new Dictionary<string, FileInfo>();
            }
            if (IsFileIncluded(folderName, _patterns))
            {
                var directoryInfoList = Directory.EnumerateDirectories(folderName);
                foreach (var directory in directoryInfoList)
                {
                    GetLocalRepository(directory, localRepository);
                }
                var filesList = Directory.EnumerateFiles(folderName);
                foreach (var file in filesList)
                {
                    if (IsFileIncluded(folderName, _patterns))
                    {
                        localRepository.Add(file, new FileInfo(file));
                        Logging.Logger.Info(string.Format("Added File to Repo: {0}", file));
                    }
                }    
            }
            return localRepository;
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
            return patterns.ToArray();
        }
    }
}

