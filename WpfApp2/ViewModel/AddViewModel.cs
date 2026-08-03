using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Vuzol.Navigation;
using Vuzol.Services;
using Vuzol.ViewModel.Command;
using Vuzol.ViewModel.Model;

namespace Vuzol.ViewModel
{
    public class AddViewModel : BaseViewModel
    {
        private RelayCommand _addPropertyCommand;
        private NavigationProperty NavigationPropertyStore { get; set; }
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
                    }
                    ));
            }
        }
        public PropertyModel PropertyAdd { get; set; }
        public string ButtonName { get; set; }
        public AddViewModel(NavigationProperty NavigationProperty, Property current, PropertyModel propertyAdd, bool isEdit = false)
        {
            HomeCommand = new NavigateCommand<HomeViewModel>(NavigationProperty,
                () => new HomeViewModel(NavigationProperty));
            PropertyAdd = propertyAdd;

            PropertyAdd.SoftwareEquipmentList = new ObservableCollection<SoftwareEquipment>(
                        SoftwareEquipmentData.GetAllSoftwareEquipment(current.InventoryNumber));
            PropertyAdd.HardwareEquipmentList = new ObservableCollection<HardwareEquipment>(
                        HardwareEquipmentData.GetAllHardwareEquipment(current.InventoryNumber));
            ButtonName = isEdit ? "Коригувати" : "Додати";
            IsEditMode = isEdit;
            NavigationPropertyStore = NavigationProperty;
        }
        public AddViewModel(NavigationProperty NavigationProperty, Property current, bool isEdit = false)
        {
            HomeCommand = new NavigateCommand<HomeViewModel>(NavigationProperty,
                () => new HomeViewModel(NavigationProperty));
            List<Employee> EmployeesList = new ObservableCollection<Employee>(EmployeeData.GetAllEmployees()).ToList();
            List<PropertyType> propertiesTypes = PropertyTypeData.GetAllPropertyType().ToList();
            List<PropertyStatus> propertyStatusList = PropertyStatusData.GetAllPropertyStatus().ToList();
            List<Employee> employeeList = EmployeeData.GetAllEmployees().ToList();
            List<MaterialResources> materialResourcesList = MaterialResourcesData.GetAllMaterialResourcess().ToList();
            List<Category> categoryList = CategoryData.GetAllCategorys().ToList();
            List<QuantityType> quantityTypeList = QuantityTypeData.GetAllQuantityTypes().ToList();
            if (isEdit)
            {              
                PropertyAdd = new PropertyModel()
                {
                    
                    IsEdit = isEdit,
                    InventoryNumber = current.InventoryNumber,
                    FactoryNumber = current.FactoryNumber,
                    Name = current.Name,
                    InvoiceId = current.InvoiceId,
                    BookId = current.BookId,
                    OrderBookId = current.OrderBookId,
                    FormId = current.FormId,
                    OrderId = current.OrderId,
                    OrderDate = current.OrderDate.ToString("yyyy/MM/dd"),
                    AdditionalInfo = current.AdditionalInfo,
                    BookPage = current.BookPage,
                    OrderBookPage = current.OrderBookPage,                   
                    Price = current.Price,
                    Quantity = current.Quantity,
                    PropertyTypes = new ObservableCollection<PropertyType>(propertiesTypes),
                    SelectedPropertyType = propertiesTypes.FirstOrDefault(a => a.Id == current.PropertyTypeId),
                    PropertyStatuses = new ObservableCollection<PropertyStatus>(propertyStatusList),
                    SelectedPropertyStatus = propertyStatusList.FirstOrDefault(a => a.Id == current.Status),
                    Employees = new ObservableCollection<Employee>(EmployeesList),
                    SelectedEmployee = EmployeesList.FirstOrDefault(a => a.Id == current.FIO_R),
                    SelectedEmployeeProvidedForUse = EmployeesList.FirstOrDefault(a => a.Id == current.FIO_V),
                    MaterialResources = new ObservableCollection<MaterialResources>(materialResourcesList),
                    SelectedMaterialResources= materialResourcesList.FirstOrDefault(a => a.Id == current.MaterialResourcesId),
                    Categories = new ObservableCollection<Category>(categoryList),
                    SelectedCategory = categoryList.FirstOrDefault(a => a.Id == current.CategoryId),
                    QuantityTypes = new ObservableCollection<QuantityType>(quantityTypeList),
                    SelectedQuantityType = quantityTypeList.FirstOrDefault(a => a.Id == current.QuantityTypeId),
                    SoftwareEquipmentList = new ObservableCollection<SoftwareEquipment>(
                        SoftwareEquipmentData.GetAllSoftwareEquipment(current.InventoryNumber)),
                    HardwareEquipmentList = new ObservableCollection<HardwareEquipment>(
                        HardwareEquipmentData.GetAllHardwareEquipment(current.InventoryNumber)),
                    AddHardwareEquipmentCommand =
                    new NavigateCommand<HardwareEquipmentViewModel>(NavigationProperty,
                        () => new HardwareEquipmentViewModel(NavigationProperty, current, PropertyAdd)),
                                        EditHardwareEquipmentCommand =
                                        new NavigateCommand<HardwareEquipmentViewModel>(NavigationProperty,
                        () => new HardwareEquipmentViewModel(NavigationProperty, current, PropertyAdd, true))
                };
            }
            else 
            {
                PropertyAdd = new PropertyModel()
                {
                    CanEditInventory = true,
                    PropertyTypes = new ObservableCollection<PropertyType>(propertiesTypes),
                    SelectedPropertyType = propertiesTypes.FirstOrDefault(a => a.Id == 0),
                    PropertyStatuses = new ObservableCollection<PropertyStatus>(propertyStatusList),
                    SelectedPropertyStatus = propertyStatusList.FirstOrDefault(a => a.Id == 0),
                    Employees = new ObservableCollection<Employee>(EmployeesList),
                    SelectedEmployee = EmployeesList.FirstOrDefault(a => a.Id == 0),
                    SelectedEmployeeProvidedForUse = EmployeesList.FirstOrDefault(a => a.Id == 0),
                    MaterialResources = new ObservableCollection<MaterialResources>(materialResourcesList),
                    SelectedMaterialResources = materialResourcesList.FirstOrDefault(a => a.Id == 0),
                    Categories = new ObservableCollection<Category>(categoryList),
                    SelectedCategory = categoryList.FirstOrDefault(a => a.Id == 0),
                    QuantityTypes = new ObservableCollection<QuantityType>(quantityTypeList),
                    SelectedQuantityType = quantityTypeList.FirstOrDefault(a => a.Id == 0),
                };
            }
            
            PropertyAdd.ErrorsChanged += (s, e) => 
            {
                _addPropertyCommand?.RaiseCanExecuteChanged();
            };

            ButtonName = isEdit ? "Коригувати" : "Додати";
            IsEditMode = isEdit;
            NavigationPropertyStore = NavigationProperty;
        }
        
        private bool IsEditMode { get; set; }
        private bool CanExecuteAddProperty(object parameter)
        {
            // Перевіряємо всі обов'язкові поля
            return string.IsNullOrEmpty(PropertyAdd.InventoryNumber);
        }

        private bool ValidateBeforeSave()
        {
            
            PropertyAdd.TriggerValidation();

            if (PropertyAdd.HasErrors)
            {
                var errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(PropertyAdd.InventoryNumber))
                    errorMessages.Add("• Інвентарний номер є обов'язковим полем");

                if (PropertyAdd.SelectedEmployee==null || PropertyAdd.SelectedEmployee.Id == 0)
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
            if (!ValidateBeforeSave())
                return null;
            Property property = GetNewProperty();   
            PropertyData.UpdateDb(property);
            return new HomeViewModel(navigationProperty);
        }
        private HomeViewModel AddNewPropertyFunc(NavigationProperty navigationProperty)
        {
            // Перевірка валідації перед збереженням
            if (!ValidateBeforeSave())
                return null;
            Property property = GetNewProperty();            
            PropertyData.InsertIntoDb(property);
            return new HomeViewModel(navigationProperty);
        }
        private Property GetNewProperty()
        {
            int propertyTypeId = PropertyAdd.SelectedPropertyType != null ? PropertyAdd.SelectedPropertyType.Id : 0; 
            int status = PropertyAdd.SelectedPropertyStatus != null ? PropertyAdd.SelectedPropertyStatus.Id : 0; 
            string UnitName = "Без підрозділу";
            int fio_r = PropertyAdd.SelectedEmployee != null ? PropertyAdd.SelectedEmployee.Id : 0;
            string Fio_r = PropertyAdd.SelectedEmployee != null ? PropertyAdd.SelectedEmployee.LastName : "";
            int fio_v = PropertyAdd.SelectedEmployeeProvidedForUse != null ? PropertyAdd.SelectedEmployeeProvidedForUse.Id : 0;
            string Fio_v = PropertyAdd.SelectedEmployeeProvidedForUse != null ? PropertyAdd.SelectedEmployeeProvidedForUse.LastName : "";
            string additionalInfo = PropertyAdd.AdditionalInfo ?? string.Empty;
            Property property = new Property(PropertyAdd.FactoryNumber, PropertyAdd.Name,
                                             PropertyAdd.InventoryNumber,
                                             PropertyAdd.InvoiceId,
                                             PropertyAdd.BookId, PropertyAdd.OrderBookId,
                                             PropertyAdd.FormId,
                                             string.Empty, DateTime.Now,
                                             PropertyAdd.OrderId, propertyTypeId,
                                             additionalInfo, DateTime.Now,
                                             fio_r, fio_v,
                                             Fio_r, Fio_v,
                                             UnitName,
                                             PropertyAdd.BookPage, PropertyAdd.OrderBookPage,
                                             DateTime.Parse(string.IsNullOrEmpty(PropertyAdd.OrderDate) ? DateTime.Now.ToString() : PropertyAdd.OrderDate),
                                             status, PropertyAdd.CategoryDescription, PropertyAdd.MaterialResourcesDescription,
                                             PropertyAdd.QuantityTypeDescription)
            {
                Quantity = PropertyAdd.Quantity,
                Price = PropertyAdd.Price,
                MaterialResourcesId = PropertyAdd.SelectedMaterialResources != null ? PropertyAdd.SelectedMaterialResources.Id : 0,
                CategoryId = PropertyAdd.SelectedCategory != null ? PropertyAdd.SelectedCategory.Id : 0,
                QuantityTypeId = PropertyAdd.SelectedQuantityType != null ? PropertyAdd.SelectedQuantityType.Id : 0,
            };
            return property;
        }
        

    }
}
