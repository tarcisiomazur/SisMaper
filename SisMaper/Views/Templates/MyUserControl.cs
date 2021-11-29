using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SisMaper.Views.Templates
{
    public class MyUserControl: UserControl
    {
        public event EventHandler? Open;
        public event EventHandler? Close;

        public void OnOpen()
        {
            if(Open != null)
                Dispatcher.BeginInvoke(DispatcherPriority.Background, Open, this, EventArgs.Empty);
        }
        
        public void OnClose()
        {
            if(Close != null)
                Dispatcher.BeginInvoke(DispatcherPriority.Background, Close, this, EventArgs.Empty);
        }
    }
}