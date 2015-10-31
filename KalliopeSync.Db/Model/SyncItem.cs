using System;

namespace KalliopeSync.Db
{
    public class SyncItem
    {
        public static string TableName = "SyncItem";
        public SyncItem()
        {
        }

        public long Id
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

