using System;
using System.Windows.Controls;

namespace SisMaper.Views.Templates
{
    public class MyUserControl: UserControl
    {
        public event EventHandler? Open;
        public event EventHandler? Close;

        public void OnOpen()
        {
            Open?.Invoke(this, EventArgs.Empty);
        }
        
        public void OnClose()
        {
            Close?.Invoke(this, EventArgs.Empty);
        }
    }
}