using System;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace SisMaper.Tools.Events;

public delegate void ShowMessageEventHandler(object? sender, ShowMessageEventArgs e);

public class ShowMessageEventArgs : EventArgs
{
    public ShowMessageEventArgs(string title, string message, MessageDialogStyle style,
        MetroDialogSettings settings = null)
    {
        Title = title;
        Message = message;
        Style = style;
        Settings = settings;
    }

    public string Title { get; set; }

    public string Message { get; set; }

    public MetroDialogSettings? Settings { get; set; }

    public MessageDialogResult Result { get; set; }

    public MessageDialogStyle Style { get; set; }

    public MessageBoxButton SystemStyle => Style switch
    {
        MessageDialogStyle.Affirmative => MessageBoxButton.OK,
        MessageDialogStyle.AffirmativeAndNegative => MessageBoxButton.YesNoCancel,
        _ => MessageBoxButton.OKCancel
    };

    public MessageBoxResult SystemResult
    {
        set
        {
            Result = value switch
            {
                MessageBoxResult.Yes or MessageBoxResult.OK => MessageDialogResult.Affirmative,
                MessageBoxResult.No => MessageDialogResult.Negative,
                MessageBoxResult.Cancel or MessageBoxResult.None => MessageDialogResult.Canceled,
                _ => default
            };
        }
    }
}