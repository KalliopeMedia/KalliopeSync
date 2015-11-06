using Gtk;

using System;
using System.Collections.Generic;

using KalliopeSync.Core.Models;

public partial class MainWindow: Gtk.Window
{
    public MainWindow()
        : base(Gtk.WindowType.Toplevel)
    {
        Build();
//        List<List<SyncItem>> items = new List<List<SyncItem>>();
//        for (int i = 0; i < 100; i++)
//        {
            List<SyncItem> subitems = new List<SyncItem>();
            for (int j = 0; j < 5; j++) 
            {
                SyncItem item = new SyncItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = string.Format("FileItem {0} - {1}", j, j),
                        Path = string.Format("/file/path/{0}/{1}", j, j)
                    };
                subitems.Add(item);
            }
//            items.Add(subitems);
//        }
        Gtk.TreeStore syncItems = new Gtk.TreeStore(typeof (SyncItem));
        for (int i = 0; i < subitems.Count; i++)
        {
            Gtk.TreeIter iter = syncItems.AppendValues (subitems[i]);
        }
        Gtk.TreeViewColumn folderNameColumn = new TreeViewColumn();
        folderNameColumn.Title = "Folders";
        Gtk.CellRendererText folderNameCell = new Gtk.CellRendererText ();
        folderNameColumn.PackStart (folderNameCell, true);
        folderNameColumn.SetCellDataFunc (folderNameCell, new Gtk.TreeCellDataFunc (RenderFolderNameCell));

        this.treeview2.AppendColumn(folderNameColumn);
        this.treeview2.Model = syncItems;

    }

    private void RenderFolderNameCell(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
    {
        SyncItem syncItem = (SyncItem) model.GetValue (iter, 0);
        (cell as Gtk.CellRendererText).Text = syncItem.Name;

    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }


}
