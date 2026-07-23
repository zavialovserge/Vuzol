using Vuzol.Navigation;

namespace Vuzol.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private NavigationProperty _navigationProperty;
        public BaseViewModel CurrentViewModel => _navigationProperty.CurrentViewModel;
        public MainViewModel(NavigationProperty NavigationProperty)
        {
            _navigationProperty = NavigationProperty;
            _navigationProperty.CurrentViewModelChanged += OnCurrentPropertyChanged;
        }

        public void OnCurrentPropertyChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
