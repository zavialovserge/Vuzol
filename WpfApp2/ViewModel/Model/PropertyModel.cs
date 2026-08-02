using Microsoft.Office.Interop.Word;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Vuzol.Services;
using Vuzol.View;
using Vuzol.ViewModel.Command;

namespace Vuzol.ViewModel.Model
{
    public class PropertyModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private RelayCommand _addSoftwareEquipmentCommand;
        private RelayCommand _editSoftwareEquipmentCommand;
        private RelayCommand _delSoftwareEquipmentCommand;
        private RelayCommand _delHardwareEquipmentCommand;
        private SoftwareEquipment _softwareEquipment;
        private HardwareEquipment _hardwareEquipment;  
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        public ObservableCollection<SoftwareEquipment> SoftwareEquipmentList { get; set; }
        public ObservableCollection<HardwareEquipment> HardwareEquipmentList { get; set; }
        private ObservableCollection<Employee> _employees;       
        private ObservableCollection<PropertyType> _propertyTypes;
        private ObservableCollection<PropertyStatus> _propertyStatuses;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<QuantityType> _quantityTypes;
        private ObservableCollection<MaterialResources> _materialResources;       
        private Employee _employee;
        private Employee _employeeProvidedForUse;
        private PropertyType _propertyType;
        private PropertyStatus _propertyStatus;
        private Category _category;
        private QuantityType _quantityType;
        private MaterialResources _materialResource;
        public ObservableCollection<Employee> Employees
        {
            get { return _employees; } 
            set
            {
                _employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }        
        public Employee SelectedEmployee
        {
            get { return _employee; }
            set
            {
                _employee = value;
                OnPropertyChanged(nameof(_employee));
            }
        }        
        public Employee SelectedEmployeeProvidedForUse
        {
            get { return _employeeProvidedForUse; }
            set
            {
                _employeeProvidedForUse = value;
                OnPropertyChanged(nameof(_employeeProvidedForUse));
            }
        }
        public ObservableCollection<PropertyType> PropertyTypes
        {
            get { return _propertyTypes; }
            set
            {
                _propertyTypes = value;
                OnPropertyChanged(nameof(PropertyTypes));
            }
        }
        public PropertyType SelectedPropertyType
        {
            get { return _propertyType; }
            set
            {
                _propertyType = value;
                OnPropertyChanged(nameof(_propertyType));
            }
        }
        public ObservableCollection<PropertyStatus> PropertyStatuses
        {
            get { return _propertyStatuses; }
            set
            {
                _propertyStatuses = value;
                OnPropertyChanged(nameof(_propertyStatuses));
            }
        }
        public PropertyStatus SelectedPropertyStatus 
        {
            get { return _propertyStatus; }
            set
            {
                _propertyStatus = value;
                OnPropertyChanged(nameof(_propertyStatus));
            }
        }
        public ObservableCollection<MaterialResources> MaterialResources
        {
            get { return _materialResources; }
            set
            {
                _materialResources = value;
                OnPropertyChanged(nameof(_materialResources));
            }
        }
        public MaterialResources SelectedMaterialResources
        {
            get { return _materialResource; }
            set
            {
                _materialResource = value;
                OnPropertyChanged(nameof(_materialResource));
            }
        }
        public ObservableCollection<QuantityType> QuantityTypes
        {
            get { return _quantityTypes; }
            set
            {
                _quantityTypes = value;
                OnPropertyChanged(nameof(_quantityTypes));
            }
        }
        public QuantityType SelectedQuantityType
        {
            get { return _quantityType; }     
            set
            {
                _quantityType = value;
                OnPropertyChanged(nameof(_quantityType));
            }
        }
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(_categories));
            }
        }
        public Category SelectedCategory
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged(nameof(_category));
            }
        }
        public SoftwareEquipment SelectedSoftwareEquipment
        {
            get { return _softwareEquipment; }
            set
            {
                _softwareEquipment = value;
                OnPropertyChanged(nameof(_softwareEquipment));
            }
        }
        public HardwareEquipment SelectedHardwareEquipment
        {
            get { return _hardwareEquipment; }
            set
            {
                _hardwareEquipment = value;
                OnPropertyChanged(nameof(_hardwareEquipment));
            }
        }
        public ICommand AddSoftwareEquipmentCommand
        {
            get
            {
                return _addSoftwareEquipmentCommand ?? (_addSoftwareEquipmentCommand = new RelayCommand(
                   property =>
                   {
                       var descr = GetSoftwareEquipmentDescription(string.Empty, 0);
                       if (descr == null) return;
                       SoftwareEquipment softwareEquipment = new SoftwareEquipment(0, FactoryNumber, descr.Item1, descr.Item2);
                       SoftwareEquipmentData.InsertSoftwareEquipment(softwareEquipment);
                       RefreshSoftwareEquipment();
                   }));
            }
        }
        public ICommand EditSoftwareEquipmentCommand
        {
            get
            {
                return _editSoftwareEquipmentCommand ?? (_editSoftwareEquipmentCommand = new RelayCommand(
                   property =>
                   {
                       var descr = GetSoftwareEquipmentDescription(SelectedSoftwareEquipment.Description, SelectedSoftwareEquipment.Quantity);
                       if (descr == null) return;
                       SelectedSoftwareEquipment.Description = descr.Item1;
                       SelectedSoftwareEquipment.Quantity = descr.Item2;
                       SoftwareEquipmentData.EditSoftwareEquipment(SelectedSoftwareEquipment);
                       RefreshSoftwareEquipment();
                   }));
            }
        }
        public ICommand DelSoftwareEquipmentCommand
        {
            get
            {
                return _delSoftwareEquipmentCommand ?? (_delSoftwareEquipmentCommand = new RelayCommand(
                   property =>
                   {
                       if (MessageBox.Show("Ви дійсно хочете видалити елемент?", "Видалити елемент",
                           MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                       {
                           return;
                       }
                       SoftwareEquipmentData.DeleteFromDb(SelectedSoftwareEquipment);
                       SoftwareEquipmentList.Remove(SelectedSoftwareEquipment);
                   }));
            }
        }
        public ICommand AddHardwareEquipmentCommand { get; set; }
        public ICommand EditHardwareEquipmentCommand { get; set; }
        public ICommand DelHardwareEquipmentCommand
        {
            get
            {
                return _delHardwareEquipmentCommand ?? (_delHardwareEquipmentCommand = new RelayCommand(
                   property =>
                   {
                       if (MessageBox.Show("Ви дійсно хочете видалити елемент?", "Видалити елемент",
                         MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                       {
                           return;
                       }
                       HardwareEquipmentData.DeleteFromDb(SelectedHardwareEquipment);
                       HardwareEquipmentList.Remove(SelectedHardwareEquipment);
                   }));
            }
        }
        private string? _additionalInfo { get; set; }
        private string _inventoryNumber { get; set; }
        private string? _factoryNumber { get; set; }
        private string? _name { get; set; }
        private int _invoiceId { get; set; }
        private int _bookId { get; set; }
        private int _orderBookId { get; set; }
        private int _formId { get; set; }
        private string? _formName { get; set; }
        private int _orderId { get; set; }
        private string _orderDate { get; set; }
        private int _bookPage { get; set; }
        private int _orderBookPage { get; set; }
        private decimal _quantity { get; set; }
        private decimal _price { get; set; }
        private string _categoryDescription { get; set; }
        private string _materialResourcesDescription { get; set; }
        private string _quantityTypeDescription { get; set; }
        public bool IsEdit { get; set; }
        public string AdditionalInfo
        {
            get { return _additionalInfo; }
            set
            {
                _additionalInfo = value;
                OnPropertyChanged(nameof(AdditionalInfo));
            }
        }
        public string? CategoryDescription
        {
            get { return _categoryDescription; }
            set
            {
                _categoryDescription = value;
                OnPropertyChanged(nameof(_categoryDescription));
            }
        }
        public string? MaterialResourcesDescription
        {
            get { return _materialResourcesDescription; }
            set
            {
                _materialResourcesDescription = value;
                OnPropertyChanged(nameof(_materialResourcesDescription));
            }
        }
        public string? QuantityTypeDescription
        {
            get { return _quantityTypeDescription; }
            set
            {
                _quantityTypeDescription = value;
                OnPropertyChanged(nameof(_quantityTypeDescription));
            }
        }
        public string InventoryNumber
        {
            get { return _inventoryNumber; }
            set
            {
                if (_inventoryNumber != value)
                {
                    _inventoryNumber = value;
                    OnPropertyChanged(nameof(InventoryNumber));
                    ValidateInventoryNumber();
                }
            }
        }
        public string? FactoryNumber
        {
            get { return _factoryNumber; }
            set
            {
                _factoryNumber = value;
                OnPropertyChanged(nameof(FactoryNumber));
            }
        }
        public string? Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(_name));
                ValidateName();
            }
        }
        private void ValidateName()
        {
            ClearErrors(nameof(Name));
            if (string.IsNullOrWhiteSpace(Name))
            {
                AddError(nameof(Name), "Найменування є обов'язковим");
            }
        }       
        public int InvoiceId
        {
            get { return _invoiceId; }
            set
            {
                _invoiceId = value;
                OnPropertyChanged(nameof(_invoiceId));
            }
        }
        public int BookId
        {
            get { return _bookId; }
            set
            {
                _bookId = value;
                OnPropertyChanged(nameof(_bookId));
            }
        }
        public int OrderBookId
        {
            get { return _orderBookId; }
            set
            {
                _orderBookId = value;
                OnPropertyChanged(nameof(_orderBookId));
            }
        }
        public int FormId
        {
            get { return _formId; }
            set
            {
                _formId = value;
                OnPropertyChanged(nameof(_formId));
            }
        }
        public string? FormName
        {
            get { return _formName; }
            set
            {
                _formName = value;
                OnPropertyChanged(nameof(_formName));
            }
        }
        public int OrderId
        {
            get { return _orderId; }
            set
            {
                _orderId = value;
                OnPropertyChanged(nameof(_orderId));
            }
        }
        public string OrderDate
        {
            get { return _orderDate; }
            set
            {
                _orderDate = value;
                OnPropertyChanged(nameof(_orderDate));
            }
        }
        public int BookPage
        {
            get { return _bookPage; }
            set
            {
                _bookPage = value;
                OnPropertyChanged(nameof(_bookPage));
            }
        }
        public int OrderBookPage
        {
            get { return _orderBookPage; }
            set
            {
                _orderBookPage = value;
                OnPropertyChanged(nameof(_orderBookPage));
            }
        }
        public decimal Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(_quantity));
            }
        }
        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(_price));
            }
        }
        // INotifyDataErrorInfo implementation
        public bool HasErrors => _errors.Any();
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public IEnumerable? GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
                return null;
            return _errors[propertyName];
        }
        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }
        private void ClearErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        // Validation methods
        private void ValidateInventoryNumber()
        {
            ClearErrors(nameof(InventoryNumber));
            if (IsEdit) return;
            if (string.IsNullOrEmpty(InventoryNumber))
            {
                AddError(nameof(InventoryNumber), "Інвентарний номер є обов'язковим");
            }
            if(PropertyData.ExistProperty(InventoryNumber))
            {
                AddError(nameof(InventoryNumber), "Інвентарний номер вже існує");
            }
        }
        private void ValidateSelectedEmployee()
        {
            ClearErrors(nameof(SelectedEmployee));
            if (SelectedEmployee == null || SelectedEmployee.Id == 0)
            {
                AddError(nameof(SelectedEmployee), "Відповідальний є обов'язковим полем");
            }
        }
        // Метод для тригерування валідації всіх обов'язкових полів
        public void TriggerValidation()
        {
            ValidateInventoryNumber();
            ValidateSelectedEmployee();
            ValidateName();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        
        private void RefreshSoftwareEquipment()
        {
            SoftwareEquipmentList.Clear();
            var softwareEquipmentData = SoftwareEquipmentData.GetAllSoftwareEquipment(FactoryNumber);
            foreach (var softwareEquipment in softwareEquipmentData)
            {
                SoftwareEquipmentList.Add(softwareEquipment);
            }
        }        
        private Tuple<string, double>? GetSoftwareEquipmentDescription(string name, double quantity)
        {
            InputDialogSample inputDialog =
                       new InputDialogSample("Введіть опис", name, true, quantity);
            if (inputDialog.ShowDialog() == false
                || string.IsNullOrEmpty(inputDialog.Answer)) return null;

            return new Tuple<string, double>(inputDialog.Answer, inputDialog.Quantity);
        }
    }
}