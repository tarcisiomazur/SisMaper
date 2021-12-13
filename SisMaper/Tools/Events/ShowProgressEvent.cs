using System;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace SisMaper.Tools.Events;

public delegate Task ShowProgressEventHandler(object? sender, ShowProgressEventArgs e);

public class ShowProgressEventArgs : EventArgs
{
    public ShowProgressEventArgs(MetroDialogSettings settings)
    {
        Settings = settings;
    }

    public ProgressDialogController? DialogController { get; set; }

    public MetroDialogSettings Settings { get; set; }
}