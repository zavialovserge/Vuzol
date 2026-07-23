
namespace Vuzol.ViewModel.Command
{
    public class EditCommand : RelayCommand
    {
        public EditCommand(Action<object> execute) : base(execute)
        {
        }

        public EditCommand(Action<object> execute, Func<object, bool> canExecute) : base(execute, canExecute)
        {
        }


    }
}