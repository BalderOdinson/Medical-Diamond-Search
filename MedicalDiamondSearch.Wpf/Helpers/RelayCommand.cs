using System;
using System.Windows.Input;

namespace MedicalDiamondSearch.Wpf.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
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
