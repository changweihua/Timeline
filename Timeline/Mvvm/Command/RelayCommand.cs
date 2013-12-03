using System;
using System.Windows.Input;
using System.Diagnostics;

namespace ShiningMeeting.Mvvm.Command
{
    public class RelayCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action<object> _execute;

        // Events
        public event EventHandler CanExecuteChanged;

        // Methods
        public RelayCommand(Action<object> execute) 
        {
            _execute = execute;
        }
        public RelayCommand(Action<object> execute, Func<bool> canExecute) 
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
            this._execute(parameter);
        }
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
