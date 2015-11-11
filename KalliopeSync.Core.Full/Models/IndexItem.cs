using System;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;

namespace KalliopeSync.Core.Models
{
    public class IndexItem 
    {
        public IndexItem()
        {

        }

        public string Id
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public long Created
        {
            get;
            set;
        }

        public long Updated
        {
            get;
            set;
        }

        public long Synced
        {
            get;
            set;
        }

        public SyncStatus Status
        {
            get;
            set;
        }

        public FileInfo File
        {
            get;
            set;
        }

        public CloudBlockBlob Blob
        {
            get;
            set;
        }

    }
}

