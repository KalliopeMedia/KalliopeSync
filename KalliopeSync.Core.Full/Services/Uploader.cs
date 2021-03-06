﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;

namespace KalliopeSync.Core.Services
{
    public class Uploader
    {
        private readonly string _accountName;
        private readonly string _accountKey;
        private readonly string _containerName;
        private readonly Dictionary<string, CloudBlockBlob> _cloudRepository;
        private readonly Dictionary<string, FileInfo> _localRepository;
        private readonly CloudStorageAccount _storageAccount;

        public int ChunkSize
        {
            get;
            set;
        }

        public bool SimulationMode
        {
            get;
            set;
        }

        public bool FullThrottle
        {
            get;
            set;
        }

        public Uploader(string userName, string accountName, string accountKey)
        {
            this._accountName = accountName;
            this._accountKey = accountKey;
            this._containerName = userName;
            this._cloudRepository = new Dictionary<string, CloudBlockBlob>();
            this._localRepository = new Dictionary<string, FileInfo>();
            this.FullThrottle = true;
            this.ChunkSize = 1024;
            var cloudConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", _accountName, _accountKey);
            _storageAccount = CloudStorageAccount.Parse(cloudConnectionString);
        }

        public void Upload(string targetFolder, IEnumerable<FileInfo> uploadList)
        {
            //var deleteList = CreateDeleteList();
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_containerName);

            Upload(uploadList, container, targetFolder);
//            Delete(deleteList);

        }

        private void Upload(IEnumerable<FileInfo> uploadList, CloudBlobContainer container, string targetFolder)
        {
            Console.WriteLine("Processing Uploads.");
            Logging.Logger.Info("Processing Uploads------------------------");
            var successCount = 0;
            foreach (var fileInfo in uploadList)
            {
                string blobReferenceName = GetBlobReferenceName(fileInfo.FullName, targetFolder);

                try
                {
                    if (!SimulationMode)
                    {
                        var blockBlob = container.GetBlockBlobReference(blobReferenceName);

                        using (var fileStream = System.IO.File.OpenRead(fileInfo.FullName))
                        {
                            if (this.FullThrottle)
                            {
                                var mB = ChunkSize * ChunkSize;
                                if (fileStream.Length > mB)
                                {
                                    var id = 1;
                                    var idList = new List<string>();
                                    Console.WriteLine($"Uploading big file {fileInfo.FullName}");
                                    while (fileStream.Position < fileStream.Length)
                                    {
                                        byte[] chunk = new byte[mB];
                                        var remainder = (int)(fileStream.Length - fileStream.Position);
                                        if(remainder < mB)
                                        {
                                            chunk = new byte[remainder];
                                            mB = remainder;
                                        }
                                        string id64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                                        Console.WriteLine(string.Format($"Reading chunk {id64} at position {fileStream.Position} of size {chunk.Length}"));
                                        fileStream.Read(chunk, 0, mB);
                                        blockBlob.PutBlock(id64, new MemoryStream(chunk), null, null, null);
                                        idList.Add(id64);
                                        id++;
                                    }
                                    blockBlob.PutBlockList(idList);
                                }
                                else
                                {
                                    blockBlob.UploadFromStream(fileStream);
                                    Logging.Logger.Info(string.Format("Uploading: File {0} to Blob Reference {1}", fileInfo.FullName, blobReferenceName));
                                }
                            }
                            else
                            {
                                byte[] chunk = new byte[ChunkSize];
                                var id = 1;
                                var idList = new List<string>();
                                while (fileStream.Position < fileStream.Length)
                                {
                                    fileStream.Read(chunk, 0, ChunkSize);
                                    string id64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                                                          string.Format(CultureInfo.InvariantCulture, "{0:D4}", id)));
                                    blockBlob.PutBlock(id64, new MemoryStream(chunk), null, null, null);
                                    idList.Add(id64);
                                    id++;
                                    Thread.Sleep(500);
                                }
                                blockBlob.PutBlockList(idList);
                                Logging.Logger.Info(string.Format("Uploaded Chunked file: File {0} to Blob Reference {1} of length {2} in {3} chunks", fileInfo.FullName, blobReferenceName, fileStream.Length, id));
                            }
                            successCount++;
                        }
                    }
                    else
                    {
                        Logging.Logger.Info(string.Format("Uploading: File {0} to Blob Reference {1}", fileInfo.FullName, blobReferenceName));
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logger.Error(string.Format("Failed to upload: File {0} to Blob Reference {1}", fileInfo.FullName, blobReferenceName), ex);
                }
            }
            Console.WriteLine($"Processing Uploads COMPLETE, {successCount} file(s) uploaded");
            Logging.Logger.Info($"Processing Uploads COMPLETE, {successCount} file(s) uploaded");

        }

        private void Delete(IEnumerable<CloudBlockBlob> deleteList)
        {
            throw new NotImplementedException();
        }

        private string GetBlobReferenceName(string fullFileName, string targetFolder)
        {
            string blobReferenceName = Path.GetFullPath(fullFileName).Replace(targetFolder, "").Replace(@"\", @"/");
            return System.Net.WebUtility.HtmlEncode(blobReferenceName);
        }
    }
}

