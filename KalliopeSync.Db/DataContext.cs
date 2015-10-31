using System;

namespace KalliopeSync.Db
{
    public class DataContext
    {
        private Provider _dataProvider;
        public DataContext(Provider dataProvider)
        {
            _dataProvider = dataProvider;
            SyncHistoryService = new SyncHistoryService(_dataProvider);
        }

        public SyncHistoryService SyncHistoryService
        {
            get;
            set;
        }
    }
}

