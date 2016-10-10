using System;

namespace KalliopeSync.View
{
    public partial class MainView : Gtk.Window
    {
        ConnectionDialog _connectionDialog;
        ViewConfig _configuration;

        public MainView()
            : base(Gtk.WindowType.Toplevel)
        {
            this.Build();
            _connectionDialog = new ConnectionDialog();
            _connectionDialog.Hide();
            _connectionDialog.Close += ConnectionDialogClosed;

            this._configuration = new ViewConfig();
        }

        void ConnectionDialogClosed (object sender, EventArgs e)
        {
            UpdateViewConfig((sender as ConnectionDialog).Configuration);
        }
           
        void UpdateViewConfig(ViewConfig config)
        {
            _configuration.AcccountKey = config.AcccountKey;
            _configuration.AccountName = config.AccountName;
            _configuration.ContainerName = config.ContainerName;
            _configuration.SyncFolder = config.SyncFolder;
        }
            
        protected void OnConnectActionActivated (object sender, EventArgs e)
        {
            
            _connectionDialog.ShowDialog(_configuration);
            Console.Write("HERE");
        }


    }
}

