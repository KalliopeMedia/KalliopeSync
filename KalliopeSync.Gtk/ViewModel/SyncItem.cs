using System;
using System.Collections.Generic;

namespace KalliopeSync.View
{
    public class SyncItem
    {
        
        public SyncItem()
        {
            this.ChildItems = new List<SyncItem>();
        }

        public List<SyncItem> ChildItems
        {
            get;
            set;
        }
        public long Id
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string FileKey
        {
            get;
            set;
        }
        public string CloudAssetId
        {
            get;
            set;
        }
        public DateTime CloudLastUpdated
        {
            get;
            set;
        }
        public DateTime CloudCreated
        {
            get;
            set;
        }
        public bool IsDeleted
        {
            get;
            set;
        }
        public long Size
        {
            get;
            set;
        }
    }
}

