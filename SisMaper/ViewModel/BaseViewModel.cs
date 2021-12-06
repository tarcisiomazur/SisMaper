﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MahApps.Metro.Controls.Dialogs;
using SisMaper.Tools.Events;

namespace SisMaper.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event ShowMessageEventHandler? ShowMessage;
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

        protected ProgressDialogController? OnShowProgressAsync(MetroDialogSettings settings = null)
        {
            var eventArgs = new ShowProgressEventArgs(settings);
            ShowProgress?.Invoke(this, eventArgs);
            return eventArgs.DialogController;
        }
    }
}