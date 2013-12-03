using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Diagnostics;

namespace ShiningMeeting.Mvvm.Command
{
    public class DelegateCommand<T> : ICommand where T : CommandParameter
    {
        private readonly Func<bool> _canExecute;
        private readonly Action<T> _execute;

        // Events
        public event EventHandler CanExecuteChanged;

        // Methods
        public DelegateCommand(Action<T> execute) 
        {
            _execute = execute;
        }
        public DelegateCommand(Action<T> execute, Func<bool> canExecute) 
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this._execute = execute;
            this._canExecute = canExecute;
        }
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (CanExecuteChanged != null) 
            {
                CanExecuteChanged(this, new EventArgs());
            }
            return ((this._canExecute == null) ? true : this._canExecute());
        }
        public void Execute(object parameter)
        {
            this._execute(parameter as T);
        }
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
