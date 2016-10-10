using System;
using Gtk;

namespace KalliopeSync.View
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            MainView win = new MainView();
            win.Show();
            win.DeleteEvent += delete_event;
            Application.Run();
        }

        static void delete_event (object obj, DeleteEventArgs args)
        {
            Application.Quit ();
        }
    }
}
