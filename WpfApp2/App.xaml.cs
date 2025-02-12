using System.Windows;
using Vuzol.Navigation;
using Vuzol.ViewModel;

namespace Vuzol
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {        
        protected override void OnStartup(StartupEventArgs e)
        { 
            NavigationProperty  navigationProperty = new NavigationProperty();
            navigationProperty.CurrentViewModel = new HomeViewModel(navigationProperty);
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(navigationProperty)
            };
            MainWindow.Show();
            base.OnStartup(e);
        }
    } 
}
