using Gtk;

using System;
using System.Collections.Generic;

using KalliopeSync.View;

public partial class MainWindow: Gtk.Window
{
    public MainWindow()
        : base(Gtk.WindowType.Toplevel)
    {
        Build();
        List<SyncItem> subitems = new List<SyncItem>();
        for (int j = 0; j < 5; j++)
        {
            SyncItem item = new SyncItem
            {
                Name = string.Format("FileItem 0 - {0}", j),
                FileKey = string.Format("/file/path/0/{0}", j)
            };
            for (int i = 0; i < 5; i++)
            {
                SyncItem subItem = new SyncItem
                    {
                        Name = string.Format("FileItem {0} - {1}", j, i),
                        FileKey = item.FileKey + string.Format("{0}", i)
                    };      
                item.ChildItems.Add(subItem);
            }

            subitems.Add(item);
        }

        Gtk.TreeStore syncItems = new Gtk.TreeStore(typeof(SyncItem));
        for (int i = 0; i < subitems.Count; i++)
        {
            var iter = syncItems.AppendValues(subitems[i]);
            for (int j = 0; j < subitems[i].ChildItems.Count; j++) {
                syncItems.AppendValues(iter, subitems[i].ChildItems[j]);
            }
        }
        Gtk.TreeViewColumn folderNameColumn = new TreeViewColumn();
        folderNameColumn.Title = "Folders";
        Gtk.CellRendererText folderNameCell = new Gtk.CellRendererText();
        folderNameColumn.PackStart(folderNameCell, true);
        folderNameColumn.SetCellDataFunc(folderNameCell, new Gtk.TreeCellDataFunc(RenderFolderNameCell));

        this.SyncLocationTreeView.AppendColumn(folderNameColumn);
        this.SyncLocationTreeView.Model = syncItems;

    }

    private void RenderFolderNameCell(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
    {
        SyncItem syncItem = (SyncItem)model.GetValue(iter, 0);
        (cell as Gtk.CellRendererText).Text = syncItem.Name;

    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }


}
