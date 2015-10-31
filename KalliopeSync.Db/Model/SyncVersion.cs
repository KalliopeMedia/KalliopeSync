using System;

namespace KalliopeSync.Db
{
    public class SyncVersion
    {
        public SyncVersion()
        {
        }

        public int Id
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        public long LastUpdatedDate
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        }
    }
}

