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

        SyncItem root = new SyncItem{ Id = DateTime.Now.Ticks, FileKey = "/root", Name = "Root" };
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                string fileKey = "/root" + string.Format("/file/path/{0}/{1}{2}",i, string.Format("FileItem {0}-", i), j); 
                KalliopeSync.Core.Models.IndexItem subItem = new KalliopeSync.Core.Models.IndexItem
                {
                    Name = string.Format("FileItem {0}", i),
                    Id = fileKey
                };      
                root.AddItem(subItem.Id, subItem);
            }
        }

        var syncItems = this.SyncLocationTreeView.DataBind(root);

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
