using System;
using MahApps.Metro.Controls.Dialogs;

namespace SisMaper.Tools.Events
{
    public delegate void ShowProgressEventHandler(object? sender, ShowProgressEventArgs e);

    public class ShowProgressEventArgs : EventArgs
    {
        public ProgressDialogController? DialogController { get; set; }
        public MetroDialogSettings Settings { get; set; }
        public ShowProgressEventArgs(MetroDialogSettings settings)
        {
            Settings = settings;
        }
    }
}