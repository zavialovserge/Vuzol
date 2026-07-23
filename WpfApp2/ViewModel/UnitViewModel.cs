using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Vuzol.Navigation;
using Vuzol.Services;
using Vuzol.View;
using Vuzol.ViewModel.Command;
using Vuzol.ViewModel.Model;

namespace Vuzol.ViewModel
{
    public class UnitViewModel : BaseViewModel
    {
        public ObservableCollection<Unit> UnitList { get; set; }
        public Unit SelectedUnit { get; set; }
        public ICommand HomeCommand { get; }
        private RelayCommand _addUnitCommand;
        private RelayCommand _editUnitCommand;
        private RelayCommand _delUnitCommand;
        public UnitViewModel(NavigationProperty navigationProperty)
        {

            HomeCommand = new NavigateCommand<HomeViewModel>(navigationProperty,
               () => new HomeViewModel(navigationProperty));
            UnitList = new ObservableCollection<Unit>(UnitData.getAllUnits());
            SelectedUnit = UnitList.First();
        }
        public ICommand DelUnitCommand
        {
            get
            {
                return _delUnitCommand ?? (_delUnitCommand = new RelayCommand(
                   x =>
                   {
                       if (MessageBox.Show("Ви дійсно хочете видалити елемент?", "Видалити елемент",
                           MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                       {
                           return;
                       }
                       UnitData.DeleteFromDb(SelectedUnit);
                       UnitList.Remove(SelectedUnit);

                   }));
            }
        }
        public ICommand AddUnitCommand
        {
            get
            {
                return _addUnitCommand ?? (_addUnitCommand = new RelayCommand(
                   x =>
                   {
                       string name = GetUnitName(string.Empty);
                       if (string.IsNullOrEmpty(name)) return;
                       Unit unit = new Unit(0, name);
                       UnitData.InsertUnit(unit);
                       RefreshCollection();

                   }));
            }
        }
        public ICommand EditUnitCommand
        {
            get
            {
                return _editUnitCommand ?? (_editUnitCommand = new RelayCommand(
                   x =>
                   {
                       string name = GetUnitName(SelectedUnit.Name);
                       if (string.IsNullOrEmpty(name)) return;
                       SelectedUnit.Name = name;
                       UnitData.EditIUnit(SelectedUnit);
                       RefreshCollection();
                   }));
            }
        }
        private string GetUnitName(string name)
        {
            InputDialogSample inputDialog =
                       new InputDialogSample("Введіть назву підрозділу:", name);
            if (inputDialog.ShowDialog() == false
                || string.IsNullOrEmpty(inputDialog.Answer)) return string.Empty;

            return inputDialog.Answer;
        }
        private void RefreshCollection()
        {
            UnitList.Clear();
            var unitData = UnitData.getAllUnits();
            foreach (var unitDb in unitData)
            {
                UnitList.Add(unitDb);
            }
        }
    }
}
