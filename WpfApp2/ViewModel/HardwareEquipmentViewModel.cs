using System.Collections.ObjectModel;
using System.Windows.Input;
using Vuzol.Navigation;
using Vuzol.Services;
using Vuzol.ViewModel.Model;

namespace Vuzol.ViewModel
{
    public class HardwareEquipmentViewModel : BaseViewModel
    {
        private string _subInventoryNumber;
        private string _description;
        private double _quantity;
        private string _previousSubFactoryNumber;
        public HardwareEquipmentViewModel(NavigationProperty NavigationProperty, Property current, PropertyModel propertyModel, bool isEdit = false)
        {
            HomeCommand = new NavigateCommand<AddViewModel>(NavigationProperty,
               () => new AddViewModel(NavigationProperty, current));
            ChangeHardwareEquipmentCommand = new NavigateCommand<AddViewModel>(NavigationProperty,
                () => isEdit ? EditHardwareEquipmentFunc(NavigationProperty, current) : AddHardwareEquipmentFunc(NavigationProperty, current));
            List<Property> subInventoryNumberList = PropertyData.GetAllProperty();
            _previousSubFactoryNumber = isEdit ? propertyModel.SelectedHardwareEquipment.SubInventoryNumber : "0";
            SubInventoryNumberList = new ObservableCollection<string>(subInventoryNumberList.Select(a => "Заводський номер:" + a.FactoryNumber));
            SubInventoryNumber = isEdit ? subInventoryNumberList.Where(a => a.FactoryNumber == _previousSubFactoryNumber)
                                                                            .Select(a => "Заводський номер:" + a.FactoryNumber)
                                                                            .First()
                                               : SubInventoryNumberList.First();
            ButtonName = isEdit ? "Коригувати" : "Додати";
            _description = isEdit ? propertyModel.SelectedHardwareEquipment.Description : "";
            _quantity = isEdit ? propertyModel.SelectedHardwareEquipment.Quantity : 0;

        }
        private AddViewModel AddHardwareEquipmentFunc(NavigationProperty navigationProperty, Property prop)
        {
            HardwareEquipment hardwareEquipment = GerNewHardwareEquipment(prop.FactoryNumber);
            HardwareEquipmentData.InsertHardwareEquipment(hardwareEquipment);
            return new AddViewModel(navigationProperty, prop);
        }
        private AddViewModel EditHardwareEquipmentFunc(NavigationProperty navigationProperty, Property prop)
        {
            HardwareEquipment hardwareEquipment = GerNewHardwareEquipment(prop.FactoryNumber);
            HardwareEquipmentData.EditHardwareEquipment(hardwareEquipment, _previousSubFactoryNumber);
            return new AddViewModel(navigationProperty, prop);
        }
        private HardwareEquipment GerNewHardwareEquipment(string factoryNumber)
        {
            string newFactoryNumber = SubInventoryNumber.Replace("Заводський номер:", string.Empty);
            return new HardwareEquipment(factoryNumber, newFactoryNumber, Quantity, Description);
        }
        public ICommand HomeCommand { get; }
        public ICommand ChangeHardwareEquipmentCommand { get; }
        public ObservableCollection<string> SubInventoryNumberList { get; set; }
        public string SubInventoryNumber
        {
            get { return _subInventoryNumber; }
            set
            {
                _subInventoryNumber = value;
                OnPropertyChanged(nameof(_subInventoryNumber));
            }
        }
        public string ButtonName { get; set; }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(_description));
            }
        }
        public double Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(_subInventoryNumber));
            }
        }
    }
}
