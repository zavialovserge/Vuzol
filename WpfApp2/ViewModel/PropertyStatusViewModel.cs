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
    public class PropertyStatusViewModel : BaseViewModel
    {
        private RelayCommand _addCommand;
        private RelayCommand _editCommand;
        private RelayCommand _delCommand;
        public PropertyStatusViewModel(NavigationProperty navigationProperty)
        {
            HomeCommand = new NavigateCommand<HomeViewModel>(navigationProperty,
               () => new HomeViewModel(navigationProperty));
            PropertyStatusList = 
                new ObservableCollection<PropertyStatus>(PropertyStatusData.GetAllPropertyStatus());
            SelectedPropertyStatus = PropertyStatusList.First();
        }
        public PropertyStatus SelectedPropertyStatus { get; set; }
        public ObservableCollection<PropertyStatus> PropertyStatusList { get; set; }
        public ICommand HomeCommand { get; }
        public ICommand DelCommand
        {
            get
            {
                return _delCommand ?? (_delCommand = new RelayCommand(
                   x =>
                   {
                       if (MessageBox.Show("Ви дійсно хочете видалити елемент?", "Видалити елемент",
                           MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                       {
                           return;
                       }
                       PropertyStatusData.DeleteFromDb(SelectedPropertyStatus);
                       PropertyStatusList.Remove(SelectedPropertyStatus);

                   }));
            }
        }
        public ICommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(
                   x =>
                   {
                       string name = GetProprertyName(string.Empty);
                       if (string.IsNullOrEmpty(name)) return;
                       PropertyStatus propertyStatus = new PropertyStatus(0, name);
                       PropertyStatusData.InsertPropertyType(propertyStatus);
                       RefreshCollection();

                   }));
            }
        }
        public ICommand EditCommand
        {
            get
            {
                return _editCommand ?? (_editCommand = new RelayCommand(
                   x =>
                   {
                       string name = GetProprertyName(SelectedPropertyStatus.Name);
                       if (string.IsNullOrEmpty(name)) return;
                       SelectedPropertyStatus.Name = name;
                       PropertyStatusData.EditPropertyType(SelectedPropertyStatus);
                       RefreshCollection();
                   }));
            }
        }
        private string GetProprertyName(string name)
        {
            InputDialogSample inputDialog =
                       new InputDialogSample("Введіть назву статус:", name);
            if (inputDialog.ShowDialog() == false
                || string.IsNullOrEmpty(inputDialog.Answer)) return string.Empty;

            return inputDialog.Answer;
        }
        private void RefreshCollection()
        {
            PropertyStatusList.Clear();
            var PropertyStatusData = Services.PropertyStatusData.GetAllPropertyStatus();
            foreach (var PropertyStatusDb in PropertyStatusData)
            {
                PropertyStatusList.Add(PropertyStatusDb);
            }
            SelectedPropertyStatus = PropertyStatusList.First();
        }
    }
}
