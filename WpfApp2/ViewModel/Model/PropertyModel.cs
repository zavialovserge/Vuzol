using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Vuzol.Services;
using Vuzol.View;
using Vuzol.ViewModel.Command;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        
        // Словник для зберігання помилок валідації
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public ObservableCollection<SoftwareEquipment> SoftwareEquipmentList { get; set; }
        public ObservableCollection<HardwareEquipment> HardwareEquipmentList { get; set; }
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
        private List<string> _propertyTypeNameList { get; set; }
        private List<string> _propertyStatusNameList { get; set; }
        private string _propertyTypeName { get; set; }
        private string _propertyStatusName { get; set; }
        private int _FIO_R { get; set; }
        private int _FIO_I { get; set; }
        private string _FIO_R_STR { get; set; }
        private string _FIO_I_STR { get; set; }
        private string? _additionalnfo { get; set; }
        private int _inventoryNumber { get; set; }
        private int _factoryNumber { get; set; }
        private string? _name { get; set; }
        private int _invoiceId { get; set; }
        private int _bookId { get; set; }
        private int _orderBookId { get; set; }
        private string? _formId { get; set; }
        private string? _formName { get; set; }
        private int _orderId { get; set; }
        private string _orderDate { get; set; }
        private int _propertyTypeId { get; set; }
        private string _unitName { get; set; }
        private int _bookPage { get; set; }
        private int _orderBookPage { get; set; }
        private int _status { get; set; }
        private decimal _quantity { get; set; }
        private decimal _price { get; set; }
        private List<string> _employeeList { get; set; }
        public bool IsEdit { get; set; }
        public int Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(_status));
            }
        }
        public string? Additionalnfo
        {
            get { return _additionalnfo; }
            set
            {
                _additionalnfo = value;
                OnPropertyChanged(nameof(_additionalnfo));
            }
        }
        public int InventoryNumber
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
        public int FactoryNumber
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
        public List<string> PropertyTypeNameList
        {
            get { return _propertyTypeNameList; }
            set
            {
                _propertyTypeNameList = value;
                OnPropertyChanged(nameof(_propertyTypeNameList));
            }
        }
        public List<string> EmployeeList
        {
            get { return _employeeList; }
            set
            {
                _employeeList = value;
                OnPropertyChanged(nameof(_employeeList));
            }
        }
        public string PropertyTypeName
        {
            get { return _propertyTypeName; }
            set
            {
                _propertyTypeName = value;
                OnPropertyChanged(nameof(_propertyTypeName));
            }
        }
        public List<string> PropertyStatusNameList
        {
            get { return _propertyStatusNameList; }
            set
            {
                _propertyStatusNameList = value;
                OnPropertyChanged(nameof(_propertyStatusNameList));
            }
        }
        public string PropertyStatusName
        {
            get { return _propertyStatusName; }
            set
            {
                _propertyStatusName = value;
                OnPropertyChanged(nameof(_propertyStatusName));
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
        public string? FormId
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
        public int PropertyTypeId
        {
            get { return _propertyTypeId; }
            set
            {
                _propertyTypeId = value;
                OnPropertyChanged(nameof(_propertyTypeId));
            }
        }
        public int FIO_R
        {
            get { return _FIO_R; }
            set
            {
                _FIO_R = value;
                OnPropertyChanged(nameof(_FIO_R));
            }
        }
        public int FIO_I
        {
            get { return _FIO_I; }
            set
            {
                _FIO_I = value;
                OnPropertyChanged(nameof(_FIO_I));
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
        public string? FIO_R_STR
        {
            get { return _FIO_R_STR; }
            set
            {
                if (_FIO_R_STR != value)
                {
                    _FIO_R_STR = value;
                    OnPropertyChanged(nameof(FIO_R_STR));
                    ValidateFIO_R_STR();
                }
            }
        }
        public string? FIO_I_STR
        {
            get { return _FIO_I_STR; }
            set
            {
                _FIO_I_STR = value;
                OnPropertyChanged(nameof(_FIO_I_STR));
            }
        }
        public string UnitName
        {
            get { return _unitName; }
            set
            {
                _unitName = value;
                OnPropertyChanged(nameof(_unitName));
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
            if (InventoryNumber <= 0)
            {
                AddError(nameof(InventoryNumber), "Інвентарний номер є обов'язковим");
            }
            if(PropertyData.ExistProperty(InventoryNumber) && !IsEdit)
            {
                AddError(nameof(InventoryNumber), "Інвентарний номер вже існує");
            }
        }

        private void ValidateFIO_R_STR()
        {
            ClearErrors(nameof(FIO_R_STR));
            if (string.IsNullOrWhiteSpace(FIO_R_STR))
            {
                AddError(nameof(FIO_R_STR), "Відповідальний є обов'язковим полем");
            }
        }

        // Метод для тригерування валідації всіх обов'язкових полів
        public void TriggerValidation()
        {
            ValidateInventoryNumber();
            ValidateFIO_R_STR();
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