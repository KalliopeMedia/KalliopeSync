using Gtk;

using System;
using System.Collections.Generic;

namespace KalliopeSync.View
{
    public partial class MainWindow: Gtk.Window
    {
        public MainWindow()
            : base(Gtk.WindowType.Toplevel)
        {
            Build();
        }
        protected void OnConnectActionActionChanged (object o, ChangedArgs args)
        {
            throw new NotImplementedException ();
        }


        protected void OnConnectActionActionChangedPataNahin (object o, ChangedArgs args)
        {
            throw new NotImplementedException ();
        }
    }
}
