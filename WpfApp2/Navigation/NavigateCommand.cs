using Vuzol.ViewModel;
using Vuzol.ViewModel.Command;

namespace Vuzol.Navigation
{

    public class NavigateCommand<TViewModel> : CommandBase where TViewModel : BaseViewModel
    {
        private readonly NavigationProperty _navigationStore;
        private readonly Func<TViewModel> _createViewModel;
        public NavigateCommand(NavigationProperty prop, Func<TViewModel> createViewModel)
        {
            _navigationStore = prop;
            _createViewModel = createViewModel;
        }
        public override void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = _createViewModel();
        }
        

    }
}
