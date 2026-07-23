using System.Windows.Input;
using Vuzol.Navigation;
using Vuzol.Services;
using Vuzol.ViewModel.Model;

namespace Vuzol.ViewModel
{
    public class AddEmployeeViewModel : BaseViewModel
    {
        public ICommand HomeCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        private bool IsEdit;
        private List<Rank> RankList;
        private List<Unit> UnitList;
        public AddEmployeeViewModel(NavigationProperty navigationProperty,
                                    Employee selectedEmployee, bool isEdit = false)
        {
            HomeCommand = new NavigateCommand<EmployeeViewModel>(navigationProperty,
               () => new EmployeeViewModel(navigationProperty));
            RankList = RankData.getAllRanks().ToList();
            UnitList = UnitData.getAllUnits().ToList();
            SelectedEmployee = isEdit ? new EmployeeModelViewModel()
            {
                Id = selectedEmployee.Id,
                FirstName = selectedEmployee.FirstName,
                LastName = selectedEmployee.LastName,
                FatherName = selectedEmployee.FatherName,
                Position = selectedEmployee.Position,
                Rank = selectedEmployee.Rank,
                RankDescription = selectedEmployee.RankDescription,
                UnitDescription = selectedEmployee.UnitName,
                RankList = RankList.Select(a => a.RankDescription).ToList(),
                UnitList = UnitList.Select(a => a.Name).ToList()
            }
                                        : new EmployeeModelViewModel()
                                        {
                                            RankList = RankList.Select(a => a.RankDescription).ToList(),
                                            UnitList = UnitList.Select(a => a.Name).ToList(),
                                            RankDescription = RankList.Count == 0
                                                                ? RankList[0].RankDescription
                                                                : "Солдат",
                                            UnitDescription = UnitList.Count == 0
                                                                ? UnitList[0].Name
                                                                : "Вузол звя'язку",
                                        };
            IsEdit = isEdit;
            ButtonName = isEdit ? "Коригувати" : "Додати";
            AddEmployeeCommand = new NavigateCommand<EmployeeViewModel>(navigationProperty,
               () => IsEdit ? EditEmployeeFunc(navigationProperty)
                            : AddNewEmployeeFunc(navigationProperty));
        }

        private EmployeeViewModel EditEmployeeFunc(NavigationProperty navigationProperty)
        {
            if (SelectedEmployee == null) return new EmployeeViewModel(navigationProperty);
            int rank = RankList
                                .FirstOrDefault(a => a.RankDescription == SelectedEmployee.RankDescription)
                                .Id;
            int unit = UnitList
                                .FirstOrDefault(a => a.Name == SelectedEmployee.UnitDescription)
                                .Id;
            Employee employee = new Employee(SelectedEmployee.Id, SelectedEmployee.FirstName,
                                             SelectedEmployee.LastName, SelectedEmployee.FatherName,
                                             rank, SelectedEmployee.Position,
                                             SelectedEmployee.RankDescription,
                                             unit, SelectedEmployee.UnitDescription);
            EmployeeData.EditIntoDb(employee);
            return new EmployeeViewModel(navigationProperty);
        }

        private EmployeeViewModel AddNewEmployeeFunc(NavigationProperty navigationProperty)
        {
            if (SelectedEmployee == null) return new EmployeeViewModel(navigationProperty);
            int rank = RankList
                                .FirstOrDefault(a => a.RankDescription == SelectedEmployee.RankDescription)
                                .Id;
            int unit = UnitList
                               .FirstOrDefault(a => a.Name == SelectedEmployee.UnitDescription)
                               .Id;
            Employee employee = new Employee(SelectedEmployee.Id, SelectedEmployee.FirstName,
                                             SelectedEmployee.LastName, SelectedEmployee.FatherName,
                                             rank, SelectedEmployee.Position,
                                             SelectedEmployee.RankDescription,
                                             unit, SelectedEmployee.UnitDescription);
            EmployeeData.InsertIntoDb(employee);
            return new EmployeeViewModel(navigationProperty);
        }

        public string ButtonName { get; set; }
        public EmployeeModelViewModel SelectedEmployee { get; }
    }
}
