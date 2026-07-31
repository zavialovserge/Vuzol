using System.Windows;
using System.Windows.Input;
using Vuzol.Navigation;
using Vuzol.Services;
using Vuzol.ViewModel.Model;
using Vuzol.ViewModel.Command;
using System.Collections.ObjectModel;

namespace Vuzol.ViewModel
{
    public class AddViewModel : BaseViewModel
    {
        private RelayCommand _addPropertyCommand;

        public AddViewModel(NavigationProperty NavigationProperty, Property current, bool isEdit = false)
        {
            HomeCommand = new NavigateCommand<HomeViewModel>(NavigationProperty,
                () => new HomeViewModel(NavigationProperty));
            
            PropertyTypeList = PropertyTypeData.GetAllPropertyType().ToList();
            PropertyStatusList = PropertyStatusData.GetAllPropertyStatus().ToList();
            EmployeesList = new ObservableCollection<Employee>(EmployeeData.GetAllEmployees());
            PropertyAdd = new PropertyModel()
            {
                PropertyTypeNameList = PropertyTypeList.Select(a => a.Name).ToList(),
                PropertyTypeName = PropertyTypeList.Where(a => a.Id == current.PropertyTypeId).First().Name,
                PropertyStatusNameList = PropertyStatusList.Select(a => a.Name).ToList(),
                PropertyStatusName = PropertyStatusList.Where(a => a.Id == current.Status).First().Name,
                Quantity = current.Quantity,
                EmployeeList = EmployeesList
                                            .Select(a => a.LastName + " " + a.FirstName)
                                            .ToList()
            };

            // Підписуємося на зміни помилок валідації
            PropertyAdd.ErrorsChanged += (s, e) => 
            {
                _addPropertyCommand?.RaiseCanExecuteChanged();
            };

            ButtonName = isEdit ? "Коригувати" : "Додати";
            IsEditMode = isEdit;
            NavigationPropertyStore = NavigationProperty;
        }

        private NavigationProperty NavigationPropertyStore { get; set; }
        private bool IsEditMode { get; set; }

        private bool CanExecuteAddProperty(object parameter)
        {
            // Перевіряємо всі обов'язкові поля
            return PropertyAdd.InventoryNumber > 0 
                && PropertyAdd.FactoryNumber > 0 
                && !string.IsNullOrWhiteSpace(PropertyAdd.FIO_R_STR);
        }

        private bool ValidateBeforeSave()
        {
            // Тригеруємо валідацію всіх обов'язкових полів
            PropertyAdd.TriggerValidation();

            if (PropertyAdd.HasErrors)
            {
                var errorMessages = new List<string>();

                if (PropertyAdd.InventoryNumber <= 0)
                    errorMessages.Add("• Інвентарний номер є обов'язковим полем");

                if (string.IsNullOrWhiteSpace(PropertyAdd.FIO_R_STR))
                    errorMessages.Add("• Відповідальний є обов'язковим полем");

                if (string.IsNullOrWhiteSpace(PropertyAdd.Name))
                    errorMessages.Add("• Назва є обов'язковим полем");

                string message = "Не можливо зберегти запис. Заповніть обов'язкові поля:\n\n" + 
                                string.Join("\n", errorMessages);

                MessageBox.Show(message, "Помилка валідації", 
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private HomeViewModel EditNewPropertyFunc(NavigationProperty navigationProperty)
        {
            // Перевірка валідації перед збереженням
            if (!ValidateBeforeSave())
                return null;

            int propertyTypeId = PropertyTypeList
                                 .Where(a => a.Name == PropertyAdd.PropertyTypeName)
                                 .First().Id;
            int status = PropertyStatusList
                                 .Where(a => a.Name == PropertyAdd.PropertyStatusName)
                                 .First().Id;
            Property property = new Property(PropertyAdd.FactoryNumber, PropertyAdd.Name,
                                             PropertyAdd.InventoryNumber,
                                             PropertyAdd.InvoiceId, PropertyAdd.BookId,
                                             PropertyAdd.OrderBookId,
                                             PropertyAdd.FormId,
                                             string.Empty, DateTime.Now,
                                             PropertyAdd.OrderId, propertyTypeId,
                                             PropertyAdd.Additionalnfo, DateTime.Now,
                                             PropertyAdd.FIO_R, PropertyAdd.FIO_I,
                                             PropertyAdd.FIO_R_STR, PropertyAdd.FIO_I_STR,
                                             PropertyAdd.UnitName,
                                             PropertyAdd.BookPage,
                                             PropertyAdd.OrderBookPage,
                                             DateTime.Parse(PropertyAdd.OrderDate), status, 
                                             PropertyAdd.CategoryDescription, PropertyAdd.MaterialResourcesDescription, 
                                             PropertyAdd.QuantityTypeDescription)
            {
                Quantity = PropertyAdd.Quantity,
                Price = PropertyAdd.Price,
                PropertyTypeName = string.IsNullOrEmpty(PropertyAdd.PropertyTypeName) ? string.Empty : PropertyAdd.PropertyTypeName,
                StatusName = string.IsNullOrEmpty(PropertyAdd.PropertyStatusName) ? string.Empty : PropertyAdd.PropertyStatusName,

            };

            PropertyData.UpdateDb(property);
            return new HomeViewModel(navigationProperty);
        }

        private HomeViewModel AddNewPropertyFunc(NavigationProperty navigationProperty)
        {
            // Перевірка валідації перед збереженням
            if (!ValidateBeforeSave())
                return null;

            int propertyTypeId = PropertyTypeList
                                 .Where(a => a.Name == PropertyAdd.PropertyTypeName)
                                 .First().Id;
            int status = PropertyStatusList
                                 .Where(a => a.Name == PropertyAdd.PropertyStatusName)
                                 .First().Id;
            Property property = new Property(PropertyAdd.FactoryNumber, PropertyAdd.Name,
                                             PropertyAdd.InventoryNumber,
                                             PropertyAdd.InvoiceId,
                                             PropertyAdd.BookId, PropertyAdd.OrderBookId,
                                             PropertyAdd.FormId,
                                             string.Empty, DateTime.Now,
                                             PropertyAdd.OrderId, propertyTypeId,
                                             PropertyAdd.Additionalnfo, DateTime.Now,
                                             PropertyAdd.FIO_R, PropertyAdd.FIO_I,
                                             PropertyAdd.FIO_R_STR, PropertyAdd.FIO_I_STR,
                                             PropertyAdd.UnitName,
                                             PropertyAdd.BookPage, PropertyAdd.OrderBookPage,
                                             DateTime.Parse(string.IsNullOrEmpty(PropertyAdd.OrderDate) ? DateTime.Now.ToString() : PropertyAdd.OrderDate), 
                                             status, PropertyAdd.CategoryDescription, PropertyAdd.MaterialResourcesDescription, 
                                             PropertyAdd.QuantityTypeDescription)
            {
                Quantity = PropertyAdd.Quantity,
                Price = PropertyAdd.Price,
                PropertyTypeName = string.IsNullOrEmpty(PropertyAdd.PropertyTypeName) ? string.Empty : PropertyAdd.PropertyTypeName,
                StatusName = string.IsNullOrEmpty(PropertyAdd.PropertyStatusName) ? string.Empty : PropertyAdd.PropertyStatusName,
            };
            PropertyData.InsertIntoDb(property);
            return new HomeViewModel(navigationProperty);
        }

        public ICommand HomeCommand { get; }
        
        public ICommand AddPropertyCommand
        {
            get
            {
                return _addPropertyCommand ?? (_addPropertyCommand = new RelayCommand(
                    param =>
                    {
                        var result = IsEditMode ? EditNewPropertyFunc(NavigationPropertyStore) : AddNewPropertyFunc(NavigationPropertyStore);
                        if (result != null)
                        {
                            NavigationPropertyStore.CurrentViewModel = result;
                        }
                    },
                    CanExecuteAddProperty));
            }
        }

        public ObservableCollection<Employee> EmployeesList { get; set; }
        public PropertyModel PropertyAdd { get; set; }
        public List<PropertyType> PropertyTypeList { get; set; }
        public List<PropertyStatus> PropertyStatusList { get; set; }
        public string ButtonName { get; set; }

    }
}
