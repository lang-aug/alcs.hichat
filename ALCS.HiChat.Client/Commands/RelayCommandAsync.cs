using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ALCS.HiChat.Client.Commands
{
    public class RelayCommandAsync : ICommand
    {
        private readonly Func<Task> execute;
        private readonly Predicate<object> canExecute;
        private bool isExecuting;

        public RelayCommandAsync(Func<Task> execute) : this(execute, null) { }

        public RelayCommandAsync(Func<Task> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (!isExecuting && canExecute == null) return true;
            return (!isExecuting && canExecute(parameter));
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public async void Execute(object parameter)
        {
            isExecuting = true;
            try { await execute(); }
            finally { isExecuting = false; }
        }
    }
}
