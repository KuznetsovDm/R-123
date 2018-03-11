using System;
using System.Windows.Input;

namespace R123.Radio
{
    class SimpleCommand<T> : ICommand
    {
        readonly Action<T> onExecute;
        public SimpleCommand(Action<T> onExecute) { this.onExecute = onExecute; }

        public event EventHandler CanExecuteChanged;
        private bool canExecute = true;
        public bool SetCanExecute
        {
            get => canExecute;
            set
            {
                if (canExecute != value)
                {
                    canExecute = value;
                    CanExecuteChanged?.Invoke(this, new CommandEventArgs(canExecute));
                }
            }
        }
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => onExecute((T)parameter);
    }

    public class CommandEventArgs : EventArgs
    {
        public readonly bool CanExecute;
        public CommandEventArgs(bool CanExecute)
        {
            this.CanExecute = CanExecute;
        }
    }
}
