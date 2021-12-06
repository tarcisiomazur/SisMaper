using System;
using MahApps.Metro.Controls.Dialogs;

namespace SisMaper.Tools.Events
{
    public delegate void ShowMessageEventHandler(object? sender, ShowMessageEventArgs e);

    public class ShowMessageEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public MetroDialogSettings Settings { get; set; }
        public MessageDialogResult Result { get; set; }
        public MessageDialogStyle Style { get; set; }

        public ShowMessageEventArgs(string title, string message, MessageDialogStyle style, MetroDialogSettings settings)
        {
            Title = title;
            Message = message;
            Style = style;
            Settings = settings;
        }

        
    }
}