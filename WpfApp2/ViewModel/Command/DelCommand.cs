using System.Windows.Input;

namespace Vuzol.ViewModel.Command
{

    public class DelCommand : CommandBase
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        public DelCommand(Action<object> execute)
        : this(execute, null)
        {
        }
        public DelCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public override void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
