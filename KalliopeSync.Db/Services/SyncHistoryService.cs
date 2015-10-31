using System;

namespace KalliopeSync.Db
{
    public class SyncHistoryService
    {
        Provider _provider;

        public SyncHistoryService(Provider dataProvider)
        {
            _provider = dataProvider;
        }
    }
}

