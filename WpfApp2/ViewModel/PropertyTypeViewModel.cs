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
    public class PropertyTypeViewModel : BaseViewModel
    {
        private RelayCommand _addCommand;
        private RelayCommand _editCommand;
        private RelayCommand _delCommand;
        public PropertyTypeViewModel(NavigationProperty navigationProperty)
        {

            HomeCommand = new NavigateCommand<HomeViewModel>(navigationProperty,
               () => new HomeViewModel(navigationProperty));
            PropertyTypeList = new ObservableCollection<PropertyType>(PropertyTypeData.GetAllPropertyType());
            SelectedPropertyType = PropertyTypeList.First();
        }
        public PropertyType SelectedPropertyType { get; set; }
        public ObservableCollection<PropertyType> PropertyTypeList  { get; set; }
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
                       PropertyTypeData.DeleteFromDb(SelectedPropertyType);
                       PropertyTypeList.Remove(SelectedPropertyType);

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
                       PropertyType propertyType = new PropertyType(0, name);
                       PropertyTypeData.InsertPropertyType(propertyType);
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
                       string name = GetProprertyName(SelectedPropertyType.Name);
                       if (string.IsNullOrEmpty(name)) return;
                       SelectedPropertyType.Name = name;
                       PropertyTypeData.EditPropertyType(SelectedPropertyType);
                       RefreshCollection();
                   }));
            }
        }      
        private string GetProprertyName(string name)
        {
            InputDialogSample inputDialog =
                       new InputDialogSample("Введіть назву мітки:", name);
            if (inputDialog.ShowDialog() == false
                || string.IsNullOrEmpty(inputDialog.Answer)) return string.Empty;

            return inputDialog.Answer;
        }
        private void RefreshCollection()
        {
            PropertyTypeList.Clear();
            var PropertyTypeData = Services.PropertyTypeData.GetAllPropertyType();
            foreach (var PropertyTypeDb in PropertyTypeData)
            {
                PropertyTypeList.Add(PropertyTypeDb);
            }
        }

    }
}
