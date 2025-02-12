using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Vuzol.Services;
using Vuzol.View;
using Vuzol.ViewModel.Command;

namespace Vuzol.ViewModel.Model
{
    public class PropertyModel : INotifyPropertyChanged
    {    
        private RelayCommand _addSoftwareEquipmentCommand;
        private RelayCommand _editSoftwareEquipmentCommand;
        private RelayCommand _delSoftwareEquipmentCommand;
        private RelayCommand _addHardwareEquipmentCommand;
        private RelayCommand _editHardwareEquipmentCommand;
        private RelayCommand _delHardwareEquipmentCommand;
        private SoftwareEquipment _softwareEquipment;
        public ObservableCollection<SoftwareEquipment> SoftwareEquipmentList { get; set; }
        public PropertyModel(int factoryNumber)
        {
            SoftwareEquipmentList = new ObservableCollection<SoftwareEquipment>(
                                    SoftwareEquipmentData.GetAllSoftwareEquipment(factoryNumber));
            //PropertyStatusList = PropertyStatusData.GetAllPropertyStatus().ToList();               
            //PropertyStatusNameList = PropertyStatusList.Select(a => a.Name).ToList(),
            //PropertyStatusName = PropertyStatusList.Where(a => a.Id == current.Status).First().Name
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
        public ICommand AddSoftwareEquipmentCommand
        {
            get
            {
                return _addSoftwareEquipmentCommand ?? (_addSoftwareEquipmentCommand = new RelayCommand(
                   property =>
                   {
                       string description = GetSoftwareEquipmentDescription(string.Empty);
                       if (string.IsNullOrEmpty(description)) return;
                       SoftwareEquipment softwareEquipment = new SoftwareEquipment(0,FactoryNumber, description);
                       SoftwareEquipmentData.InsertSoftwareEquipment(softwareEquipment);
                       RefreshCollection();
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
                       string description = GetSoftwareEquipmentDescription(SelectedSoftwareEquipment.Description);
                       if (string.IsNullOrEmpty(description)) return;
                       SelectedSoftwareEquipment.Description = description;
                       SoftwareEquipmentData.EditSoftwareEquipment(SelectedSoftwareEquipment);
                       RefreshCollection();
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
        public ICommand AddHardwareEquipmentCommand
        {
            get
            {
                return _addHardwareEquipmentCommand ?? (_addHardwareEquipmentCommand = new RelayCommand(
                   property =>
                   {

                   }));
            }
        }
        public ICommand EditHardwareEquipmentCommand
        {
            get
            {
                return _editHardwareEquipmentCommand ?? (_editHardwareEquipmentCommand = new RelayCommand(
                   property =>
                   {

                   }));
            }
        }
        public ICommand DelHardwareEquipmentCommand
        {
            get
            {
                return _delHardwareEquipmentCommand ?? (_delHardwareEquipmentCommand = new RelayCommand(
                   property =>
                   {

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
        private int _inventoryNumberStr { get; set; }
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
        private List<string> _employeeList { get; set; }

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
        public int InventoryNumberStr
        {
            get { return _inventoryNumberStr; }
            set
            {
                _inventoryNumberStr = value;
                OnPropertyChanged(nameof(_inventoryNumberStr));
            }
        }
        public int FactoryNumber
        {
            get { return _factoryNumber; }
            set
            {
                _factoryNumber = value;
                OnPropertyChanged(nameof(_factoryNumber));
            }
        }
        public string? Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(_name));
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
                _FIO_R_STR = value;
                OnPropertyChanged(nameof(_FIO_R_STR));
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
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void RefreshCollection()
        {
            SoftwareEquipmentList.Clear();
            var softwareEquipmentData = SoftwareEquipmentData.GetAllSoftwareEquipment(FactoryNumber);
            foreach (var softwareEquipment in softwareEquipmentData)
            {
                SoftwareEquipmentList.Add(softwareEquipment);
            }
        }

        private string GetSoftwareEquipmentDescription(string name)
        {
            InputDialogSample inputDialog =
                       new InputDialogSample("Введіть опис", name);
            if (inputDialog.ShowDialog() == false
                || string.IsNullOrEmpty(inputDialog.Answer)) return string.Empty;

            return inputDialog.Answer;
        }
    }
}