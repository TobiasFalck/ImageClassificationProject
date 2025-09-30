using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfImageClassification.ViewModel
{
    public class BaseCommand : ICommand
    {
        protected Action<object> _executeAction;
        protected Func<object, bool> _canExecuteAction;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public BaseCommand(Action<object> executeAction, Func<object, bool> canExecuteAction)
        {
            _executeAction = executeAction;
            _canExecuteAction = canExecuteAction;
        }

        public BaseCommand(Action<object> executeAction)
        {
            _executeAction = executeAction;
        }

        public bool CanExecute(object? parameter)
        {
            Boolean returnVal = true;

            if (_canExecuteAction != null)
                returnVal = _canExecuteAction(parameter);

            return returnVal;
        }

        public void Execute(object? parameter)
        {
            _executeAction?.Invoke(parameter);
        }
    }
}
