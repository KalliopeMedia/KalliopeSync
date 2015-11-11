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
        //SyncItem subitems = new List<SyncItem>();
        SyncItem root = new SyncItem{ Id = DateTime.Now.Ticks, FileKey = "root", Name = "Root" };
        for (int j = 0; j < 5; j++)
        {
            string fileKey = "/root" + string.Format("/file/path/0/{0}{1}", root,j);   
            var subRoot = root.AddItem(fileKey, new KalliopeSync.Core.Models.IndexItem{ Id = fileKey,Name= "Subroot" + j.ToString()});
 
            for (int i = 0; i < 5; i++)
            {
                KalliopeSync.Core.Models.IndexItem subItem = new KalliopeSync.Core.Models.IndexItem
                    {
                        Name = string.Format("FileItem {0}", i),
                        Id = root.FileKey + string.Format("{0}{1}", i,j)
                    };      
                subRoot.AddItem(subItem.Id, subItem);
            }
        }

        Gtk.TreeStore syncItems = new Gtk.TreeStore(typeof(SyncItem));
        //for (int i = 0; i < subitems.Count; i++)
        {
            var iter = syncItems.AppendValues(root);
            foreach (var item in root.ChildItems)
            {
                var rootIter = syncItems.AppendValues(iter, item.Value);
                foreach (var subItem in item.Value.ChildItems) {
                    syncItems.AppendValues(rootIter, subItem.Value);

                }
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
