using System;

namespace KalliopeSync.View
{
    public partial class MainView : Gtk.Window
    {
        ConnectionDialog _connectionDialog;

        public MainView()
            : base(Gtk.WindowType.Toplevel)
        {
            this.Build();
        }

        protected void OnAfterConnectActionActivated (object sender, EventArgs e)
        {
            _connectionDialog = new ConnectionDialog();

            _connectionDialog.Show();
        }

        protected void MainWindowDestroyEvent (object o, Gtk.DestroyEventArgs args)
        {
            _connectionDialog.Destroy();
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        protected void MainWindowDeleteEvent (object o, Gtk.DeleteEventArgs args)
        {
            
        }
    }
}

