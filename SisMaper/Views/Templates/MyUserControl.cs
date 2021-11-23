using System;
using System.Windows.Controls;

namespace SisMaper.Views.Templates
{
    public class MyUserControl: UserControl
    {
        public EventHandler? OnOpen;
        public EventHandler? OnClose;
    }
}