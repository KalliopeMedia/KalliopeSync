using System;
using System.Collections.Generic;

namespace KalliopeSync.View
{
    public class SyncItem
    {
        
        public SyncItem()
        {
            this.ChildItems = new Dictionary<string, SyncItem>();
        }

        public Dictionary<string, SyncItem> ChildItems
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

        public SyncItem AddItem(string path, KalliopeSync.Core.Models.IndexItem indexItem)
        {
            string [] pathStructure = path.Replace('\\','/').Split('/');
            string key = pathStructure[0];
            if (this.ChildItems.ContainsKey(key))
            {
                return this.ChildItems[key].AddItem(path.Replace(@"/" + key, ""), indexItem);
            }
            else
            {
                var newItem = new SyncItem { Id = DateTime.Now.Ticks, FileKey = indexItem.Path };
                newItem.Name = key;
                this.ChildItems.Add(path, newItem);
                return newItem;
            }
        }
    }
}

