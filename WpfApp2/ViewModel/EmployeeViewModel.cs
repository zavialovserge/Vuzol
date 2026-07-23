using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Vuzol.Navigation;
using Vuzol.Services;
using Vuzol.ViewModel.Command;
using Vuzol.ViewModel.Model;

namespace Vuzol.ViewModel
{
    public class EmployeeViewModel : BaseViewModel
    {
        public ObservableCollection<Employee> EmployeesList { get; set; }
        public ObservableCollection<Rank> RankList { get; set; }
        public Employee SelectedEmployee { get; set; }
        public ICommand HomeCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand EditEmployeeyCommand { get; }
        private RelayCommand _delEmployeeCommand;
        public EmployeeViewModel(NavigationProperty NavigationProperty)
        {
            EmployeesList = new ObservableCollection<Employee>(EmployeeData.GetAllEmployees());
            SelectedEmployee = EmployeesList.First();
            AddEmployeeCommand = new NavigateCommand<AddEmployeeViewModel>(NavigationProperty,
                                                () => new AddEmployeeViewModel(NavigationProperty,
                                                SelectedEmployee));
            EditEmployeeyCommand = new NavigateCommand<AddEmployeeViewModel>(NavigationProperty,
                                                () => new AddEmployeeViewModel(NavigationProperty,
                                                SelectedEmployee, true));

            HomeCommand = new NavigateCommand<HomeViewModel>(NavigationProperty,
               () => new HomeViewModel(NavigationProperty));

        }

        public ICommand DelEmployeeCommand
        {
            get
            {
                return _delEmployeeCommand ?? (_delEmployeeCommand = new RelayCommand(
                   x =>
                   {
                       if (MessageBox.Show("Ви дійсно хочете видалити елемент?", "Видалити елемент",
                           MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                       {
                           return;
                       }
                       if (SelectedEmployee != null)
                       {
                           if (!EmployeeData.DeleteFromDb(SelectedEmployee)) return;
                           EmployeesList.Remove(SelectedEmployee);
                       }
                   }));
            }
        }

    }
}
