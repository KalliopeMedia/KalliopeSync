using System;
using System.Collections.Generic;

using KalliopeSync.View;

namespace KalliopeSync.View
{
    public static class TreeViewExtensions
    {
        public static Gtk.TreeStore DataBind(this Gtk.TreeView tree, SyncItem data)
        {

            Gtk.TreeStore syncItems = new Gtk.TreeStore(typeof(SyncItem));

            var iter = syncItems.AppendValues(data);
            SetChildItems(syncItems, iter, data.ChildItems.Values);
 
            return syncItems;
        }

        private static void SetChildItems(Gtk.TreeStore syncItems, Gtk.TreeIter iter, IEnumerable<SyncItem> items)
        {
            foreach (var item in items)
            {
                var childIter = syncItems.AppendValues(iter, item);
                if (item.ChildItems.Count > 0)
                {
                    SetChildItems(syncItems, childIter, item.ChildItems.Values);
                }
            }
        }

    }
}
