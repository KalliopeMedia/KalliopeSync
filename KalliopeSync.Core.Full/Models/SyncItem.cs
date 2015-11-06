using System;

namespace KalliopeSync.Core.Models
{
    public class SyncItem 
    {
        public SyncItem()
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
    }
}

