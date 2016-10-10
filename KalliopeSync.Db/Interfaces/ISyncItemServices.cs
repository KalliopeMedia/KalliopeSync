using System;
using System.Collections.Generic;

namespace KalliopeSync.Db
{
    public interface ISyncItemServices
    {
        IList<SyncItem> GetAll();
        SyncItem GetById(long id);
        SyncItem Save(SyncItem item);
        SyncItem CreateSyncItem(SyncItem newItem);
        SyncItem UpdateSyncItem(SyncItem updatedItem);
        bool DeleteSyncItem(long syncItemId);
    }
}

