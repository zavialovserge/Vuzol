
namespace Vuzol.ViewModel.Command
{
    public class AddCommand : RelayCommand
    {
        public AddCommand(Action<object> execute) : base(execute)
        {
        }

        public AddCommand(Action<object> execute, Func<object, bool> canExecute) : base(execute, canExecute)
        {
        }

    }
}
