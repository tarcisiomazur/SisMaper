using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SisMaper.ViewModel
{
    public abstract class BaseCommand : ICommand
    {
        //public event EventHandler CanExecuteChanged;


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
