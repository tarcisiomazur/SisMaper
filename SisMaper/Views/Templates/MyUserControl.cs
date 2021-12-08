using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SisMaper.Views.Templates
{
    public class MyUserControl: UserControl
    {
        public Window? Window => Window.GetWindow(this);
        public event EventHandler? Show;
        public event EventHandler? Hide;

        public void OnShow()
        {
            if(Show != null)
                Dispatcher.BeginInvoke(DispatcherPriority.Background, Show, this, EventArgs.Empty);
        }
        
        public void OnHide()
        {
            if(Hide != null)
                Dispatcher.BeginInvoke(DispatcherPriority.Background, Hide, this, EventArgs.Empty);
        }
    }
}