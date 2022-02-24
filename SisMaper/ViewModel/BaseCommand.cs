using System;
using System.Windows.Input;
using SisMaper.Tools;

namespace SisMaper.ViewModel;

public class FullCmd<T> : ICommand
{
    public FullCmd(Action<T?> execute = null, Predicate<T?> canExecute = null)
    {
        CanExecuteDelegate = canExecute;
        ExecuteDelegate = execute;
    }

    public FullCmd(Action<T?> execute, Func<bool> canExecute)
    {
        CanExecuteDelegate = _ => canExecute();
        ExecuteDelegate = execute;
    }

    public Predicate<T?>? CanExecuteDelegate { get; set; }

    public Action<T?> ExecuteDelegate { get; set; }

    public bool CanExecute(object? parameter)
    {
        return CanExecuteDelegate == null || CanExecuteDelegate(parameter is T ? (T) parameter : default);
    }

    public void Execute(object? parameter)
    {
        ExecuteDelegate?.Invoke(parameter is T ? (T) parameter : default);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}

public class SimpleCommand : ICommand
{
    public SimpleCommand(Action<object> execute = null, Predicate<object> canExecute = null)
    {
        CanExecuteDelegate = canExecute;
        ExecuteDelegate = execute;
    }

    public SimpleCommand(Action? execute, Func<bool> canExecute)
    {
        CanExecuteDelegate = _ => canExecute();
        ExecuteDelegate = _ => execute?.Invoke();
    }

    public SimpleCommand(Action<object> execute, Func<bool> canExecute)
    {
        CanExecuteDelegate = _ => canExecute();
        ExecuteDelegate = execute;
    }

    public SimpleCommand(Action? execute = null, Predicate<object>? canExecute = null)
    {
        CanExecuteDelegate = canExecute;
        ExecuteDelegate = _ => execute?.Invoke();
    }

    public Predicate<object> CanExecuteDelegate { get; set; }

    public Action<object> ExecuteDelegate { get; set; }

    public bool CanExecute(object parameter)
    {
        return CanExecuteDelegate == null || CanExecuteDelegate(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public void Execute(object parameter)
    {
        try
        {
            ExecuteDelegate?.Invoke(parameter);
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
            throw;
        }
    }
}