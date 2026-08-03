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
    public class QuantityTypeViewModel:BaseViewModel
    {

        public ObservableCollection<QuantityType> QuantityTypeList { get; set; }
        public QuantityType SelectedQuantityType { get; set; }
        public ICommand HomeCommand { get; }
        private RelayCommand _addQuantityTypeCommand;
        private RelayCommand _editQuantityTypeCommand;
        private RelayCommand _delQuantityTypeCommand;
        public QuantityTypeViewModel(NavigationProperty navigationProperty)
        {
            HomeCommand = new NavigateCommand<HomeViewModel>(navigationProperty,
               () => new HomeViewModel(navigationProperty));
            QuantityTypeList = new ObservableCollection<QuantityType>(QuantityTypeData.GetAllQuantityTypes());
            SelectedQuantityType = QuantityTypeList.First();
        }
        public ICommand DelQuantityTypeCommand
        {
            get
            {
                return _delQuantityTypeCommand ?? (_delQuantityTypeCommand = new RelayCommand(
                   x =>
                   {
                       if (MessageBox.Show("Ви дійсно хочете видалити елемент?", "Видалити елемент",
                           MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                       {
                           return;
                       }
                       QuantityTypeData.DeleteFromDb(SelectedQuantityType);
                       QuantityTypeList.Remove(SelectedQuantityType);

                   }));
            }
        }
        public ICommand AddQuantityTypeCommand
        {
            get
            {
                return _addQuantityTypeCommand ?? (_addQuantityTypeCommand = new RelayCommand(
                   x =>
                   {
                       string decription = GetQuantityTypeName(string.Empty);
                       if (string.IsNullOrEmpty(decription)) return;
                       QuantityType QuantityType = new QuantityType(0, decription);
                       QuantityTypeData.InsertQuantityType(QuantityType);
                       RefreshCollection();

                   }));
            }
        }
        public ICommand EditQuantityTypeCommand
        {
            get
            {
                return _editQuantityTypeCommand ?? (_editQuantityTypeCommand = new RelayCommand(
                   x =>
                   {
                       string name = GetQuantityTypeName(SelectedQuantityType.Description);
                       if (string.IsNullOrEmpty(name)) return;
                       SelectedQuantityType.Description = name;
                       QuantityTypeData.EditIEank(SelectedQuantityType);
                       RefreshCollection();
                   }));
            }
        }
        private string GetQuantityTypeName(string name)
        {
            InputDialogSample inputDialog =
                       new InputDialogSample("Введіть одиниці виміру", name);
            if (inputDialog.ShowDialog() == false
                || string.IsNullOrEmpty(inputDialog.Answer)) return string.Empty;

            return inputDialog.Answer;
        }
        private void RefreshCollection()
        {
            QuantityTypeList.Clear();
            var quantityTypeData = QuantityTypeData.GetAllQuantityTypes();
            foreach (var quantityType in quantityTypeData)
            {
                QuantityTypeList.Add(quantityType);
            }
        }
    }
}
