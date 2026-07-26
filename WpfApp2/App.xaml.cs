using System.Windows;
using System.Windows.Threading;
using Vuzol.Navigation;
using Vuzol.ViewModel;
using WpfApp2.Services;

namespace Vuzol
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Глобальне перехоплення необроблених винятків у UI потоці
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Глобальне перехоплення необроблених винятків у фонових потоках
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Глобальне перехоплення винятків у Task
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            NavigationProperty navigationProperty = new NavigationProperty();
            navigationProperty.CurrentViewModel = new HomeViewModel(navigationProperty);
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(navigationProperty)
            };
            MainWindow.Show();
            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ErrorLogger.LogError(e.Exception, "Необроблений виняток у UI потоці");
            
            MessageBox.Show(
                $"Виникла критична помилка:\n\n{e.Exception.Message}\n\nДеталі збережено в лог-файл.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Встановіть true, щоб додаток продовжував працювати після помилки
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                ErrorLogger.LogError(exception, $"Необроблений виняток у домені додатку. Завершення: {e.IsTerminating}");
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            ErrorLogger.LogError(e.Exception, "Необроблений виняток у Task");
            e.SetObserved(); // Запобігає завершенню програми
        }
    }
}
