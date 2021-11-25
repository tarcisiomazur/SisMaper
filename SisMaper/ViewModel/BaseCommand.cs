using System;
using System.Windows.Input;

namespace SisMaper.ViewModel
{
    public class SimpleCommand : BaseCommand
    {
        public SimpleCommand(Action<object> execute = null, Predicate<object> canExecute = null)
        {
            CanExecuteDelegate = canExecute;
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
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public override void Execute(object parameter)
        {
            this.ExecuteDelegate?.Invoke(parameter);
        }
    }

    public abstract class BaseCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public virtual bool CanExecute(object parameter) => true;
        public abstract void Execute(object parameter);


        /*
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        */
    }
}