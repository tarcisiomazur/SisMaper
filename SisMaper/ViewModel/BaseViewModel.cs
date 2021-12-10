using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using SisMaper.Tools.Events;

namespace SisMaper.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event ShowMessageEventHandler? ShowMessage;
        public event ShowInputEventHandler? ShowInput;
        public event ShowProgressEventHandler? ShowProgress;

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                RaisePropertyChanged(propertyName);
            }
        }


        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected MessageDialogResult OnShowMessage(string title, string message, MessageDialogStyle style = default, MetroDialogSettings settings = null)
        {
            var eventArgs = new ShowMessageEventArgs(title, message, style, settings);
            ShowMessage?.Invoke(this, eventArgs);
            return eventArgs.Result;
        }

        protected async Task<ProgressDialogController?> OnShowProgressAsync(MetroDialogSettings settings = null)
        {
            var eventArgs = new ShowProgressEventArgs(settings);
            if (ShowProgress?.Invoke(this, eventArgs) is { } task)
                await task;
            return eventArgs.DialogController;
        }

        protected string? OnInput(string title, string message, MetroDialogSettings settings = null)
        {
            var eventArgs = new ShowInputEventArgs(title, message, settings);
            ShowInput?.Invoke(this, eventArgs);
            return eventArgs.Result;
        }
    }
}