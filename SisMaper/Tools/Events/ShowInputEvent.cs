using System;
using MahApps.Metro.Controls.Dialogs;

namespace SisMaper.Tools.Events;

public delegate void ShowInputEventHandler(object? sender, ShowInputEventArgs e);

public class ShowInputEventArgs : EventArgs
{
    public ShowInputEventArgs(string title, string message, MetroDialogSettings settings)
    {
        Title = title;
        Message = message;
        Settings = settings;
    }

    public string Title { get; set; }

    public string Message { get; set; }

    public MetroDialogSettings Settings { get; set; }

    public string? Result { get; set; }
}