using System;

namespace KalliopeSync.Core
{
    public enum SyncStatus
    {
        Unknown = 0,
        InSync = 1,
        UploadPending = 2,
        DownloadPending = 3,
        LocalDeletePending = 4,
        CloudDeletePending = 5
    }
}

