using System;

namespace KalliopeSync.View
{
    public partial class ConnectionDialog : Gtk.Dialog
    {
        public ViewConfig Configuration
        {
            get;
            set;
        }

        public ConnectionDialog()
        {
            this.Build();
            Configuration = new ViewConfig();
        }

        public void ShowDialog(ViewConfig config)
        {
            this.Configuration = config;
            this.SetDataBind();
            this.Show();
        }

        private void SetDataBind()
        {
            this.TextViewAccount.Buffer.Text = this.Configuration.AccountName;
            this.TextViewAccountKey.Buffer.Text = this.Configuration.AcccountKey;
            this.TextViewContainer.Buffer.Text = this.Configuration.ContainerName;
            this.TextViewSyncFolder.Buffer.Text = this.Configuration.SyncFolder;
        }

        protected void OnButtonOkClicked (object sender, EventArgs e)
        {
            this.Configuration.AccountName = this.TextViewAccount.Buffer.Text;
            this.Configuration.AcccountKey = this.TextViewAccountKey.Buffer.Text;
            this.Configuration.ContainerName = this.TextViewContainer.Buffer.Text;
            this.Configuration.SyncFolder = this.TextViewSyncFolder.Buffer.Text;
        }
    }
}

