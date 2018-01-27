using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicalDiamondSearch.Uwp.Helpers
{
    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public async void Execute(object parameter)
        {
            await _execute();
        }

        private event EventHandler CanExecuteChanged;

        event EventHandler ICommand.CanExecuteChanged
        {
            add => this.CanExecuteChanged += value;
            remove => this.CanExecuteChanged -= value;
        }

        public virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
