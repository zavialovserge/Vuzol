using System.Collections.ObjectModel;
using System.Windows.Input;
using Vuzol.Navigation;
using Vuzol.Services;
using Vuzol.ViewModel.Model;

namespace Vuzol.ViewModel
{
    public class AddViewModel:BaseViewModel 
    {
        public AddViewModel(NavigationProperty NavigationProperty,Property current,bool isEdit=false)
        {
            HomeCommand = new NavigateCommand<HomeViewModel>(NavigationProperty, 
                () => new HomeViewModel(NavigationProperty));
            AddPropertyCommand = new NavigateCommand<HomeViewModel>(NavigationProperty,
                () => isEdit ? EditNewPropertyFunc(NavigationProperty) : AddNewPropertyFunc(NavigationProperty));
            PropertyTypeList = PropertyTypeData.GetAllPropertyType().ToList();
            PropertyStatusList = PropertyStatusData.GetAllPropertyStatus().ToList();
            EmployeesList = new ObservableCollection<Employee>(EmployeeData.GetAllEmployees());
            PropertyAdd = new PropertyModel()
            {
                InventoryNumberStr = current.InventoryNumber,
                FactoryNumber = current.FactoryNumber,
                Name = current.Name,
                InvoiceId = current.InvoiceId,
                BookId = current.BookId,
                OrderBookId = current.OrderBookId,
                FormId = current.FormId,
                OrderId = current.OrderId,
                OrderDate = current.OrderDate.ToString("yyyy/MM/dd"),
                PropertyTypeId = current.PropertyTypeId,
                Additionalnfo = current.Additionalnfo,
                UnitName = current.UnitName,
                BookPage = current.BookPage,
                Status = current.Status,
                OrderBookPage = current.OrderBookPage,
                FIO_R_STR = current.FIO_R_STR,
                FIO_R = current.FIO_R,
                PropertyTypeNameList = PropertyTypeList.Select(a => a.Name).ToList(),
                PropertyTypeName = PropertyTypeList.Where(a => a.Id == current.PropertyTypeId).First().Name,
                PropertyStatusNameList = PropertyStatusList.Select(a => a.Name).ToList(),
                PropertyStatusName = PropertyStatusList.Where(a => a.Id == current.Status).First().Name,
                EmployeeList = EmployeesList
                                            .Select(a=>a.LastName + " " + a.FirstName)
                                            .ToList(),
                SoftwareEquipmentList = new ObservableCollection<SoftwareEquipment>(
                                    SoftwareEquipmentData.GetAllSoftwareEquipment(current.FactoryNumber)),
                HardwareEquipmentList = new ObservableCollection<HardwareEquipment>(
                                    HardwareEquipmentData.GetAllHardwareEquipment(current.FactoryNumber)),
                AddHardwareEquipmentCommand = new NavigateCommand<HardwareEquipmentViewModel>(NavigationProperty,
                () => new HardwareEquipmentViewModel(NavigationProperty, current, PropertyAdd)),
                EditHardwareEquipmentCommand = new NavigateCommand<HardwareEquipmentViewModel>(NavigationProperty,
                () => new HardwareEquipmentViewModel(NavigationProperty, current, PropertyAdd,true)),
            };
            ButtonName = isEdit ? "Коригувати" : "Додати";
            
        }
        private HomeViewModel EditNewPropertyFunc(NavigationProperty navigationProperty)
        {
            int propertyTypeId = PropertyTypeList
                                 .Where(a => a.Name == PropertyAdd.PropertyTypeName)
                                 .First().Id;
            int status = PropertyStatusList
                                 .Where(a => a.Name == PropertyAdd.PropertyStatusName)
                                 .First().Id;
            Property property = new Property(PropertyAdd.FactoryNumber, PropertyAdd.Name,
                                             PropertyAdd.InventoryNumberStr,
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
                                             DateTime.Parse(PropertyAdd.OrderDate), status);

            PropertyData.UpdateDb(property);
            return new HomeViewModel(navigationProperty);
        }
        private HomeViewModel AddNewPropertyFunc(NavigationProperty navigationProperty)
        {
            int propertyTypeId = PropertyTypeList
                                 .Where(a => a.Name == PropertyAdd.PropertyTypeName)
                                 .First().Id;
            int status = PropertyStatusList
                                 .Where(a => a.Name == PropertyAdd.PropertyStatusName)
                                 .First().Id;
            Property property = new Property(PropertyAdd.FactoryNumber, PropertyAdd.Name,
                                             PropertyAdd.InventoryNumberStr,
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
                                             DateTime.Parse(PropertyAdd.OrderDate), status);
            PropertyData.InsertIntoDb(property);
            return new HomeViewModel(navigationProperty);
        }
        public ICommand HomeCommand { get; }
        public ICommand AddPropertyCommand { get; }
        public ObservableCollection<Employee> EmployeesList { get; set; }
        public PropertyModel PropertyAdd { get; set; }      
        public List<PropertyType> PropertyTypeList { get; set; }
        public List<PropertyStatus> PropertyStatusList { get; set; }
        public string ButtonName { get; set; }
        
    }
}
