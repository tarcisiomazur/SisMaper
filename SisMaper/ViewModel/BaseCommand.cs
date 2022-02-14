using System;
using System.Windows.Input;
using SisMaper.Tools;

namespace SisMaper.ViewModel;

public class FullCmd<T> : ICommand
{
    public FullCmd(Action<T> execute = null, Predicate<T> canExecute = null)
    {
        CanExecuteDelegate = canExecute;
        ExecuteDelegate = execute;
    }

    public FullCmd(Action<T> execute, Func<bool> canExecute)
    {
        CanExecuteDelegate = _ => canExecute();
        ExecuteDelegate = execute;
    }

    public Predicate<T>? CanExecuteDelegate { get; set; }

    public Action<T>? ExecuteDelegate { get; set; }

    public bool CanExecute(object? parameter)
    {
        return CanExecuteDelegate == null || parameter is not T tParameter || CanExecuteDelegate(tParameter);
    }

    public void Execute(object? parameter)
    {
        if (parameter is T tParameter) ExecuteDelegate?.Invoke(tParameter);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}

public class SimpleCommand : BaseCommand
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

    public override bool CanExecute(object parameter)
    {
        return CanExecuteDelegate == null || CanExecuteDelegate(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public override void Execute(object parameter)
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

public abstract class BaseCommand : ICommand
{
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public virtual bool CanExecute(object parameter)
    {
        return true;
    }

    public abstract void Execute(object parameter);

    /*
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
    */
}